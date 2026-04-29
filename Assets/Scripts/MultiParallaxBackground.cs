using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    [Tooltip("背景Sprite/物体Transform")]
    public Transform layerTrans;

    [Tooltip("该图层视差倍率（偏离系数，远景小、近景大）")]
    public float parallaxFactor;

    [Tooltip("该图层单向最大偏移距离")]
    public float layerMaxOffsetX;
    public float layerMaxOffsetY;

    [HideInInspector] public Vector3 originPos;
}

public class MultiParallaxBackground : MonoBehaviour
{
    public static MultiParallaxBackground Instance;
    [Header("全局视差设置")]
    public ParallaxLayer[] parallaxLayers;

    [Header("全局鼠标限制")]
    [Tooltip("鼠标最大影响比例(0~1)，1=全部拉满，所有图层同时到极限")]
    [Range(0f, 1f)] public float globalMaxRatio = 0.35f;

    [Tooltip("视差移动平滑插值")]
    public float smoothSpeed = 6f;

    [Tooltip("能够移动背景")]
    public bool _isControlParallax = true;

    private Vector2 _currentMouseRatio;

    void Start()
    {
        Instance = this;
        // 记录所有图层初始位置
        foreach (var layer in parallaxLayers)
        {
            if (layer.layerTrans != null)
                layer.originPos = layer.layerTrans.position;
        }

        // 初始隐藏鼠标
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // ESC 切换鼠标&UI模式
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SwitchCursorMode();
        }

        if (_isControlParallax)
        {
            CalculateMouseRatio();
            UpdateAllParallaxLayer();
        }
    }

    // 计算鼠标相对于屏幕中心的 归一化比例 -1~1
    void CalculateMouseRatio()
    {
        // 解锁状态下用屏幕坐标，锁光标也兼容
        Vector2 mouseScreenPos = Input.mousePosition;

        // 转为屏幕中心为原点 -0.5 ~ 0.5
        float ratioX = (mouseScreenPos.x - Screen.width * 0.5f) / Screen.width;
        float ratioY = (mouseScreenPos.y - Screen.height * 0.5f) / Screen.height;

        // 限制全局最大比例，关键：全体共用一个封顶值
        ratioX = Mathf.Clamp(ratioX, -globalMaxRatio, globalMaxRatio);
        ratioY = Mathf.Clamp(ratioY, -globalMaxRatio, globalMaxRatio);

        _currentMouseRatio = new Vector2(ratioX, ratioY);
    }

    // 统一更新所有图层位置
    void UpdateAllParallaxLayer()
    {
        foreach (var layer in parallaxLayers)
        {
            if (layer.layerTrans == null) continue;

            // 核心：全局比例 * 图层独立系数
            float offsetX = _currentMouseRatio.x * layer.parallaxFactor;
            float offsetY = _currentMouseRatio.y * layer.parallaxFactor;

            // 限制该图层自身最大偏移
            offsetX = Mathf.Clamp(offsetX, -layer.layerMaxOffsetX, layer.layerMaxOffsetX);
            offsetY = Mathf.Clamp(offsetY, -layer.layerMaxOffsetY, layer.layerMaxOffsetY);

            // 平滑移动
            Vector3 targetPos = new Vector3(
                layer.originPos.x + offsetX,
                layer.originPos.y + offsetY,
                layer.layerTrans.position.z
            );

            layer.layerTrans.position = Vector3.Lerp(
                layer.layerTrans.position,
                targetPos,
                smoothSpeed * Time.deltaTime
            );
        }
    }

    void SwitchCursorMode()
    {
        _isControlParallax = !_isControlParallax;
        if (_isControlParallax)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}