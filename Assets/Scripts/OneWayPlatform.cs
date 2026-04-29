using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    [Header("穿透忽略时长")]
    public float ignoreCollisionTime = 0.3f;
    [Header("碰撞体")]
    public Collider2D solidCollider;
    private float ignoreTimer;
    private bool isIgnoring;

    void Awake()
    {
        // 获取上层实体碰撞体（非Trigger）
        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (var c in cols)
        {
            if (!c.isTrigger)
            {
                solidCollider = c;
                break;
            }
        }
    }

    void Update()
    {
        if (isIgnoring)
        {
            ignoreTimer -= Time.deltaTime;
            if (ignoreTimer <= 0f)
            {
                StopIgnoreCollision();
            }
        }
    }

    // 临时忽略平台碰撞（下方上来 / 主动下跳）
    public void StartIgnoreCollision()
    {
        if (solidCollider == null) return;

        isIgnoring = true;
        ignoreTimer = ignoreCollisionTime;
        solidCollider.enabled = false;
    }

    void StopIgnoreCollision()
    {
        isIgnoring = false;
        if (solidCollider != null)
            solidCollider.enabled = true;
    }
}