using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("玩家移动速度")]
    public float moveSpeed = 8f;

    [Tooltip("玩家跳跃力度")]
    public float jumpForce = 16f;

    [Header("地面检测")]
    [Tooltip("地面检测点（空物体挂在角色脚下）")]
    public Transform groundCheck;
    [Tooltip("地面检测半径")]
    public float groundCheckRadius = 0.2f;
    [Tooltip("地面图层（选中地面所在的Layer）")]
    public LayerMask groundLayer;

    // 组件引用
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // 状态判断
    private bool isGrounded;

    void Awake()
    {
        // 获取挂载的组件
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 检测是否在地面上
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 跳跃输入（空格键 / 触摸屏幕 / 手机触屏）
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // 翻转角色朝向
        FlipPlayer();
    }

    void FixedUpdate()
    {
        // 物理移动（FixedUpdate中处理物理逻辑）
        MovePlayer();
    }

    /// <summary>
    /// 玩家左右移动
    /// </summary>
    void MovePlayer()
    {
        // 获取水平输入（A/D ←→ 均支持）
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // 设置水平速度，保持垂直速度不变
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }

    /// <summary>
    /// 玩家跳跃
    /// </summary>
    void Jump()
    {
        // 先重置垂直速度，防止双重跳跃力度
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// 根据移动方向翻转角色
    /// </summary>
    void FlipPlayer()
    {
        if (rb.velocity.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (rb.velocity.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    // 绘制地面检测范围（编辑器中可见）
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}