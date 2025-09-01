using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Castle : BuildingBase
{

    [Header("UI")]
    [SerializeField] private Text infoText;


    void Start()
    {
        Init();
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    public override void UpdateUI()
    {
        if (infoText != null)
        {
            // infoText.text = $"Gold: {currentGold}\nWorkers: {workers.Count}/{maxWorkers}";
        }
    }

    public bool TryEnter(Worker worker)
    {

        return worker.Team == Team;
    }


    void OnDestroy()
    {

    }
}
