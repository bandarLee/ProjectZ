using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    // ��� ������� ����
    private Dictionary<string, GameObject> weapons = new Dictionary<string, GameObject>();

    public static WeaponManager Instance;

    public GameObject axeGameObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // ���⸦ ������ �߰�
    public void RegisterWeapon(string weaponName, GameObject weaponObject)
    {
        if (!weapons.ContainsKey(weaponName))
        {
            weapons.Add(weaponName, weaponObject);
            weaponObject.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ
        }
    }

    // ���� Ȱ��ȭ(��� ���� ��Ȱ��ȭ �� ���� ���⸸ Ȱ��ȭ)
    public void EquipWeapon(string weaponName)
    {
        foreach (var weapon in weapons)
        {
            weapon.Value.SetActive(false);
        }

        // ���õ� ���⸸ Ȱ��ȭ
        if (weapons.ContainsKey(weaponName))
        {
            weapons[weaponName].SetActive(true);
        }
        else
        {
            Debug.LogError($"Weapon '{weaponName}' not registered in Weapon Manager.");
        }
    }
}
