using UnityEngine;

//[RequireComponent(typeof(Collider2D))]
public class PlatformGroupRotator : MonoBehaviour
{
    [Header("子平台")]
    public Transform[] childPlatforms;
    public bool autoGetChilds = true;

    [Header("是否有玩家在平台上")]
    public bool playerOnPlatform = false;
    private bool lastPlayerOnPlatform;

    [Header("平台角度")]
    public bool isSetMaxAngleLimit = false;

    // 旋转灵敏度（可在Inspector面板调整）
    [Header("旋转设置")]
    [Tooltip("旋转灵敏度，值越大旋转越快")]
    public float rotateSensitivity = 1f;

    // 私有变量：记录鼠标起始角度、是否正在旋转、物体初始角度
    private float _startMouseAngle;
    private float _startObjectRotation;
    private bool _isRotating = false;

    void Start()
    {
        //GetComponent<Collider2D>().isTrigger = true;
        if (autoGetChilds)
        {
            childPlatforms = GetComponentsInChildren<Transform>();
        }
        lastPlayerOnPlatform = playerOnPlatform;
    }

    void Update()
    {
        if (playerOnPlatform)
        {
            if (!lastPlayerOnPlatform) 
            {
                StartRecordRotation();
            }
        }
        lastPlayerOnPlatform = playerOnPlatform;

        // 子平台强制保持水平
        KeepChildHorizontal();

        UpdateRotationAngle();
    }

    /// <summary>
    /// 核心调用函数：每次调用时，初始化旋转起始数据（从0开始计算旋转角度）
    /// </summary>
    public void StartRecordRotation()
    {
        // 1. 获取鼠标当前位置相对于物体的世界坐标
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseWorldPos - (Vector2)transform.position;

        // 2. 计算鼠标起始角度（从0度开始记录）
        _startMouseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // 确保角度为正（0~360度）
        if (_startMouseAngle < 0) _startMouseAngle += 360f;

        // 3. 记录物体当前的旋转角度（用于叠加计算）
        _startObjectRotation = transform.eulerAngles.z;

        // 4. 标记开始旋转
        _isRotating = true;
    }

    void UpdateRotationAngle() 
    {
        // 仅在标记旋转时执行计算
        if (!_isRotating) return;

        // 1. 获取当前鼠标相对于物体的方向
        Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 currentDir = currentMousePos - (Vector2)transform.position;

        // 2. 计算当前鼠标角度
        float currentMouseAngle = Mathf.Atan2(currentDir.y, currentDir.x) * Mathf.Rad2Deg;
        if (currentMouseAngle < 0) currentMouseAngle += 360f;

        // 3. 计算鼠标旋转差值（核心：从起始0角度开始的旋转量）
        float angleDelta = currentMouseAngle - _startMouseAngle;

        // 4. 叠加差值到物体旋转（Z轴为2D旋转轴）
        float finalRotation = _startObjectRotation + angleDelta * rotateSensitivity;
        transform.rotation = Quaternion.Euler(0, 0, finalRotation);
    }
    /// <summary>
    /// 停止旋转（可选，调用后停止计算角度）
    /// </summary>
    public void StopRotation()
    {
        _isRotating = false;
    }

    // 子平台保持水平
    void KeepChildHorizontal()
    {
        foreach (var t in childPlatforms)
        {
            if (t != transform)
            {
                t.rotation = Quaternion.identity;
            }
        }
    }
}