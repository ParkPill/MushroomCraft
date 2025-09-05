using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BtnMenu : MonoBehaviour
{
    Image _img;
    bool _isInit = false;
    public UnitTypes SpawnUnit;
    public List<UnitTypes> OrConditions = new List<UnitTypes>();
    bool _isConditionSet = false;
    void Start()
    {
        Init();
        CheckConditions();
    }
    void SetCondition()
    {
        if (_isConditionSet) return;
        _isConditionSet = true;
        if (SpawnUnit == UnitTypes.EngineeringBay)
        {
            OrConditions.Add(UnitTypes.Turret);
        }
        else if (SpawnUnit == UnitTypes.Barracks)
        {
            OrConditions.Add(UnitTypes.Turret);
        }
    }
    void Init()
    {
        if (_isInit) return;
        _isInit = true;
        _img = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    public void CheckConditions()
    {
        Init();
        SetCondition();
        print($"CheckConditions : {name}");
        GameScript gameScript = GameManager.Instance.TheGameScript;
        if (gameScript == null) return;
        foreach (var condition in OrConditions)
        {
            if (!gameScript.AllUnitList.Any(x => x.UnitType == condition && x.Team == gameScript.MyTeam))
            {
                // print($"condition not pass : {condition}/{name}");
                _img.material = GameManager.Instance.TheGameScript.GrayMaterial;
                foreach (Transform child in transform)
                {
                    Image childImg = child.GetComponent<Image>();
                    if (childImg) childImg.material = GameManager.Instance.TheGameScript.GrayMaterial;
                }
                return;
            }
        }
        _img.material = null;
        foreach (Transform child in transform)
        {
            Image childImg = child.GetComponent<Image>();
            if (childImg) childImg.material = null;
        }
    }

    public void OnClick()
    {
        UnitTypes missingCondition = UnitTypes.Worker;
        if (OrConditions.Count > 0)
        {
            missingCondition = OrConditions.FirstOrDefault(x => !GameManager.Instance.TheGameScript.AllUnitList.Any(y => y.UnitType == x && y.Team == GameManager.Instance.TheGameScript.MyTeam));
        }
        print($"missingCondition : {missingCondition}/{name}");
        if (missingCondition != UnitTypes.Worker)
        {
            GameManager.Instance.TheGameScript.TheUIScript.ShowInstantMessage(missingCondition.ToString());
            return;
        }
        GameManager.Instance.TheGameScript.OnMenuButtonClick(SpawnUnit);
        // if (gameObject.name.Contains("btnSpawn"))
        // {
        //     GameManager.Instance.TheGameScript.OnSpawnClick(gameObject);
        // }
        // else if (gameObject.name.Contains("btnMagic"))
        // {
        //     GameManager.Instance.TheGameScript.OnMagicClick(gameObject);
        // }
    }

}
