using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour, IInteractable
{
    [Header("������ ����� ��������")]
    public List<ItemData> shopItems = new List<ItemData>();

    [Header("��� �������� (�����������)")]
    public string merchantName = "��������";

    public void Interact()
    {
        if (UIManagerShop.Instance != null)
        {
            UIManagerShop.Instance.OpenMerchantShop(shopItems);
        }
    }
}