using UnityEngine;
using System.Collections.Generic;
using System;

public class BreakableMathWall : MonoBehaviour, IInteractable
{
    [Header("��������� �����������")]
    public string requiredToolName = "IronPickaxe";

    [Header("�������������� ������ �����")]
    public List<MathTimingLevel> wallLevels = new List<MathTimingLevel>();

    [Header("��� �������� ����� ����������")]
    [Tooltip("�������� ���� ��� �������, ������� ������ �������� (�����, �������� � �.�.)")]
    public List<GameObject> objectsToDestroy = new List<GameObject>();

    [Header("�������")]
    public GameObject breakEffect;
    public AudioClip breakSound;

    public void Interact()
    {
        ItemData tool = InventorySystem.Instance.GetEquippedTool();

        if (tool != null && tool.itemName == requiredToolName)
        {
            MathTimingMinigame.Instance.StartMinigame(wallLevels, OnMinigameSuccess);
        }
        else
        {
            Debug.Log($"�����: {requiredToolName}");
        }
    }

    private void OnMinigameSuccess()
    {
        if (breakEffect) Instantiate(breakEffect, transform.position, Quaternion.identity);
        if (breakSound && SoundManager.instance) SoundManager.instance.PlaySound(breakSound);

        if (objectsToDestroy.Count > 0)
        {
            foreach (GameObject obj in objectsToDestroy)
            {
                if (obj != null) Destroy(obj);
            }
        }
        else
        {
            Destroy(gameObject);
        }

        Debug.Log("����� � ��������� ������� ����������!");
    }
}