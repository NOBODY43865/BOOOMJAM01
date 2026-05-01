using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    [Header("移动设置")]
    public float moveSpeed = 8f;
    public float jumpForce = 10f;

    [Header("地面检测")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("动画渲染")]
    public SpriteRenderer spriteRenderer;

    //private bool isUpToDown = false;
    private Rigidbody2D rb;
    private bool isGrounded;
    // 当玩家站在平台上时，保存平台父物体
    private Transform _platformParent;

    [Header("脚下平台种类")]
    public OneWayPlatform currentOneWayPlatform;
    public PlatformGroupRotator currentPlatformGroupRotator;

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        CheckGround();

        // ========== 关键修改：按住下 直接穿透平台 ==========
        if (Input.GetAxisRaw("Vertical") < -0.5f && currentOneWayPlatform != null)
        {
            //UnityEngine.Debug.Log("触发s");
            currentOneWayPlatform.StartIgnoreCollision();
            //isUpToDown = true;
            //Invoke(nameof(SetisUpToDownFalse), 0.5f);
        }

        // 正常跳跃（只在地面可以跳）
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        FlipPlayer();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(h * moveSpeed, rb.velocity.y);
    }

    void CheckGround() 
    {
        // 1. 地面圆形检测
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 没着地直接清空平台
        if (!isGrounded)
        {
            ExitPlatform();
            return;
        }

        // 2. 向下射线检测脚下地面 (起点、方向、距离、层级)
        float rayDistance = groundCheckRadius;
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, rayDistance, groundLayer);

        // 射线命中且碰到旋转平台
        if (hit)
        {
            PlatformGroupRotator platform = hit.collider.transform.GetComponentInParent<PlatformGroupRotator>();
            if (platform != null && currentPlatformGroupRotator != platform)
            {
                EnterPlatform(platform);
            }
        }
        else
        {
            ExitPlatform();
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void FlipPlayer()
    {
        if (rb.velocity.x < -0.1f)
            spriteRenderer.flipX = true;
        else if (rb.velocity.x > 0.1f)
            spriteRenderer.flipX = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out OneWayPlatform platform) )//&& !isUpToDown)
        {
            currentOneWayPlatform = platform;
            // 从下往上跳自动穿过平台
            if (rb.velocity.y > 0)
            {
                platform.StartIgnoreCollision();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out OneWayPlatform p) && currentOneWayPlatform == p && other == currentOneWayPlatform.solidCollider)
        {
            currentOneWayPlatform = null;
        }
    }

    //private void SetisUpToDownFalse() 
    //{
    //    isUpToDown = false;
    //}

    // 进入旋转平台
    void EnterPlatform(PlatformGroupRotator platform)
    {
        UnityEngine.Debug.Log("感应到站在旋转平台上");
        currentPlatformGroupRotator = platform;
        MultiParallaxBackground.Instance._isControlParallax = false;
        currentPlatformGroupRotator.playerOnPlatform = true;
    }

    // 离开旋转平台
    void ExitPlatform()
    {
        if (currentPlatformGroupRotator == null) return;

        UnityEngine.Debug.Log("感应到离开旋转平台");
        MultiParallaxBackground.Instance._isControlParallax = true;
        currentPlatformGroupRotator.playerOnPlatform = false;
        currentPlatformGroupRotator.StopRotation();
        currentPlatformGroupRotator = null;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // 检测碰撞对象是移动平台（标签为Platform）
        if (other.gameObject.CompareTag("Platform"))
        {
            // 关键：将玩家设为平台的子物体 → 自动跟随移动
            _platformParent = other.transform;
            transform.SetParent(_platformParent);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        // 玩家离开平台 → 取消父子关系
        if (other.gameObject.CompareTag("Platform"))
        {
            transform.SetParent(null);
            _platformParent = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position,groundCheckRadius);
        }
    }
}