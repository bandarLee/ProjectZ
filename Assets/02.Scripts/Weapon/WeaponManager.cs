using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    // 모든 무기들을 저장
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

    // 무기를 사전에 추가
    public void RegisterWeapon(string weaponName, GameObject weaponObject)
    {
        if (!weapons.ContainsKey(weaponName))
        {
            weapons.Add(weaponName, weaponObject);
            weaponObject.SetActive(false); // 초기에는 비활성화
        }
    }

    // 무기 활성화(모든 무기 비활성화 후 선택 무기만 활성화)
    public void EquipWeapon(string weaponName)
    {
        foreach (var weapon in weapons)
        {
            weapon.Value.SetActive(false);
        }

        // 선택된 무기만 활성화
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
