using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BookTrigger : MonoBehaviour
{
    public TextMeshProUGUI MissionText;

    public void UpdateMissionText(string newText)
    {
        if (MissionText != null)
        {
            MissionText.text = newText;
        }
    }
}
