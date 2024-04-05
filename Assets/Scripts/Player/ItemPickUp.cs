using UnityEngine;

// ��Ʒʰȡ�ű�
public class ItemPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>(); // ��ȡ��ײ���ϵ���Ʒ���

        if (item != null)
        {
            // ��ȡ��Ʒ����
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            // �����Ʒ���Ա�ʰȡ
            if (itemDetails.canBePickedUp == true)
            {
                // ����Ʒ��ӵ�������
                InventoryManager.Instance.AddItem(InventoryLocation.player, item, collision.gameObject);

                // ����ʰȡ��Ч
                AudioManager.Instance.PlaySound(SoundName.effectPickupSound);
            }
        }
    }
}