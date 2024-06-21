using UnityEngine;
using UMA;
using UMA.CharacterSystem;

public class CharacterCustomization : MonoBehaviour
{
    public DynamicCharacterAvatar avatar;
    public UMAData umaData;
    public string myRecipe;
    public void Start()
    {
        avatar = GetComponent<DynamicCharacterAvatar>();
        umaData = GetComponent<UMAData>();
    }
    // ���� ���� �޼���
    public void ChangeColor(string colorName, Color color)
    {
        avatar.SetColor(colorName, color);
        avatar.BuildCharacter();
    }

    // �Ǻλ� ���� ����
    public void ChangeSkinColor(Color newSkinColor)
    {
        ChangeColor("Skin", newSkinColor);
    }

    // �Ӹ��� ���� ����
    public void ChangeHairColor(Color newHairColor)
    {
        ChangeColor("Hair", newHairColor);
    }

    // �� �� ���� ����
    public void ChangeEyeColor(Color newEyeColor)
    {
        ChangeColor("Eyes", newEyeColor);
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            ChangeSkinColor(Color.red);
            Debug.Log("Rde");
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ChangeSkinColor(Color.blue);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ChangeSkinColor(Color.black);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ChangeSlot("Hair", "ShavedHead");
            myRecipe = avatar.GetCurrentRecipe();
            Debug.Log(myRecipe);
            avatar.ClearSlots();
            avatar.LoadFromRecipeString(myRecipe);
        }
    }
    public void ChangeSlot(string slotName, string assetName)
    {
        avatar.SetSlot(slotName, assetName);
        avatar.BuildCharacter();
        myRecipe = avatar.GetCurrentRecipe();
        Debug.Log("Updated Recipe: " + myRecipe);
    }
}
