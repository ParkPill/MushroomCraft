using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barracks : BuildingBase
{
    void Start()
    {
        Init();
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
        float dt = Time.deltaTime;
        UpdateSmoke(dt);
        UpdateProduction(dt);
    }

    public override void UpdateUI()
    {
        if (infoText != null)
        {
            // infoText.text = $"Gold: {currentGold}\nWorkers: {workers.Count}/{maxWorkers}";
        }
    }

    void OnDestroy()
    {

    }
}
