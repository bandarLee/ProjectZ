using UnityEngine;
using UMA;
using UMA.CharacterSystem;

public static class UMAHelper
{
    // UMAData���� DNA ������ ���ڿ��� ��ȯ
    public static string GetDNAString(UMAData umaData)
    {
        if (umaData != null)
        {
            var umaDna = umaData.umaRecipe.GetDna<UMADnaHumanoid>();
            if (umaDna != null)
            {
                return JsonUtility.ToJson(umaDna);
            }
        }
        return null;
    }

    // DNA ���ڿ��� UMAData�� ����
    public static void ApplyDNAString(UMAData umaData, string dnaString)
    {
        if (umaData != null && !string.IsNullOrEmpty(dnaString))
        {
            var umaDna = umaData.umaRecipe.GetDna<UMADnaHumanoid>();
            if (umaDna != null)
            {
                JsonUtility.FromJsonOverwrite(dnaString, umaDna);
                umaData.Dirty();
            }
        }
    }
}
