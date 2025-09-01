using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BtnMenu : MonoBehaviour
{
    Image _img;
    bool _isInit = false;
    public List<UnitTypes> OrConditions = new List<UnitTypes>();
    // Start is called before the first frame update
    void Start()
    {
        Init();
        CheckConditions();
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
        GameScript gameScript = GameManager.Instance.TheGameScript;
        if (gameScript == null) return;
        foreach (var condition in OrConditions)
        {
            if (gameScript.SelectedList.Any(x => x.UnitType == condition))
            {
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
            missingCondition = OrConditions.FirstOrDefault(x => !GameManager.Instance.TheGameScript.SelectedList.Any(y => y.UnitType == x));
        }
        if (missingCondition != UnitTypes.Worker)
        {
            GameManager.Instance.TheGameScript.TheUIScript.ShowInstantMessage(missingCondition.ToString());
            return;
        }
        if (gameObject.name.Contains("btnBuild"))
        {
            GameManager.Instance.TheGameScript.OnBuildClick(gameObject);
        }
        else if (gameObject.name.Contains("btnHTBuild"))
        {
            GameManager.Instance.TheGameScript.OnHTBuildClick(gameObject);
        }
        else if (gameObject.name.Contains("btnSpawn"))
        {
            GameManager.Instance.TheGameScript.OnSpawnClick(gameObject);
        }
        else if (gameObject.name.Contains("btnMagic"))
        {
            GameManager.Instance.TheGameScript.OnMagicClick(gameObject);
        }
    }
    
}
