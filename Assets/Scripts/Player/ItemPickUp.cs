using UnityEngine;

// 物品拾取脚本
public class ItemPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>(); // 获取碰撞体上的物品组件

        if (item != null)
        {
            // 获取物品详情
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            // 如果物品可以被拾取
            if (itemDetails.canBePickedUp == true)
            {
                // 将物品添加到背包中
                InventoryManager.Instance.AddItem(InventoryLocation.player, item, collision.gameObject);

                // 播放拾取音效
                AudioManager.Instance.PlaySound(SoundName.effectPickupSound);
            }
        }
    }
}