using UnityEngine;

/// <summary>
/// 鼠标控制多层视差背景偏移 + ESC切换鼠标显示/隐藏
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [Header("视差背景设置")]
    [Tooltip("按顺序放入背景层，越靠后的层速度越大（偏移越明显）")]
    public BackgroundLayer[] backgroundLayers;

    [Header("鼠标灵敏度")]
    [Tooltip("鼠标移动对背景的影响强度")]
    public float mouseSensitivity = 1f;

    [Header("鼠标控制")]
    [Tooltip("游戏开始时是否自动隐藏鼠标")]
    public bool autoHideMouse = true;

    // 私有变量
    private Vector2 _mouseStartPos;
    private bool _isMouseControllingBackground = true;

    [System.Serializable]
    public class BackgroundLayer
    {
        [Tooltip("背景物体")]
        public Transform background;

        [Tooltip("偏移速度，数值越大偏移越明显（前景设大，背景设小）")]
        public float moveSpeed;

        [Tooltip("X轴最大偏移量（防止背景飘出屏幕）")]
        public float maxOffsetX;

        [Tooltip("Y轴最大偏移量")]
        public float maxOffsetY;

        // 记录原始位置
        [HideInInspector]
        public Vector3 originPos;
    }

    void Start()
    {
        // 记录所有背景的初始位置
        foreach (var layer in backgroundLayers)
        {
            layer.originPos = layer.background.position;
        }

        // 游戏启动自动隐藏鼠标
        if (autoHideMouse)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        // 按 ESC 切换鼠标状态（控制背景 / 控制UI）
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SwitchMouseMode();
        }

        // 只有鼠标隐藏时，才用鼠标控制背景偏移
        if (_isMouseControllingBackground)
        {
            ParallaxMove();
        }
    }

    /// <summary>
    /// 视差偏移核心逻辑
    /// </summary>
    void ParallaxMove()
    {
        // 获取鼠标移动量（增量）
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        foreach (var layer in backgroundLayers)
        {
            // 计算目标偏移 = 鼠标方向 * 每层速度
            float targetX = layer.background.position.x + mouseX * layer.moveSpeed;
            float targetY = layer.background.position.y + mouseY * layer.moveSpeed;

            // 限制偏移范围（不超过最大极限值）
            targetX = Mathf.Clamp(targetX, layer.originPos.x - layer.maxOffsetX, layer.originPos.x + layer.maxOffsetX);
            targetY = Mathf.Clamp(targetY, layer.originPos.y - layer.maxOffsetY, layer.originPos.y + layer.maxOffsetY);

            // 应用新位置
            layer.background.position = new Vector3(targetX, targetY, layer.background.position.z);
        }
    }

    /// <summary>
    /// 切换鼠标模式：隐藏控制背景 / 显示控制UI
    /// </summary>
    void SwitchMouseMode()
    {
        _isMouseControllingBackground = !_isMouseControllingBackground;

        if (_isMouseControllingBackground)
        {
            // 隐藏鼠标，锁定视角
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // 显示鼠标，释放控制
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}