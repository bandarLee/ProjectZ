using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TheLastYggdrasilWave : MonoBehaviour
{
    public GameObject TheLastYggdrasilPrefab;
    public Slider TheLastYggdrasilHPSlider;

    private void Start()
    {
        TheLastYggdrasilHPSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        
    }
}
