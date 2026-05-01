using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    [Header("弹跳力度")]
    public float bouncePower = 18f;

    private void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("触发OnCollisionEnter2D");
        if (!col.collider.CompareTag("Player")) 
        {
            Debug.Log("不是Player退出函数");
            return; 
        }

        // 获取碰撞接触点
        foreach (var contact in col.contacts)
        {
            Debug.Log("已经遍历其上物品");
            // 接触点法线朝上 = 玩家从上方踩下来
            //if (contact.normal.y > 0.5f)
            //{
                Debug.Log("触发法线判断");
                Rigidbody2D rb = col.collider.GetComponent<Rigidbody2D>();
                rb.velocity = new Vector2(rb.velocity.x, 0);
                Debug.Log("触发弹跳");
                rb.AddForce(Vector2.up * bouncePower, ForceMode2D.Impulse);
                break;
            //}
        }
    }
}