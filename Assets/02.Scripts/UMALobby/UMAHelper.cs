using UnityEngine;
using UMA;
using UMA.CharacterSystem;

public static class UMAHelper
{
    // UMAData에서 DNA 정보를 문자열로 변환
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

    // DNA 문자열을 UMAData에 적용
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
