using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using StudioNAP;

public class UIScript : MonoBehaviour
{
    public Transform MiniMap;
    // public Transform LeftBottom;
    public Camera MiniMapCamera;
    // public Transform RightTop;
    public NavMeshSurface TheSurface;
    public List<Transform> _previousMessages = new List<Transform>();
    public string _lastMsg = "";
    public Transform TopCanvas;
    GameScript _gameScript;
    public Transform Btns;
    bool _updateCurrentRequested = false;
    private Transform _topBar;
    public bool IsBuildSelected = false;
    public bool IsHighTechBuildSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        _gameScript = GetComponent<GameScript>();
        _topBar = TopCanvas.Find("TopBar");

        float width = _gameScript.MapBoundsMax.x - _gameScript.MapBoundsMin.x;
        float height = _gameScript.MapBoundsMax.y - _gameScript.MapBoundsMin.y;
        MiniMapCamera.orthographicSize = (width > height ? width : height) / 2;
        MiniMapCamera.transform.position = _gameScript.MapBoundsMin + new Vector3(width / 2, height / 2, MiniMapCamera.transform.position.z);

        UpdateMiniMapRectSize();
        UpdateMiniMapRectPosition();

        // MiniMap.Find("imgFrame").GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
    }
    public void UpdateMenuBox(List<UnitBase> unitList)
    {
        print("UpdateMenuBox: " + unitList.Count);
        foreach (Transform btn in Btns)
        {
            btn.gameObject.SetActive(false);
        }

        if (IsBuildSelected)
        {
            foreach (var building in unitList[0].GetComponent<Worker>().BuildingList)
            {
                Transform btn = Btns.Find("btnBuild" + building);
                if (btn) btn.gameObject.SetActive(true);
                else
                {
                    CreateMenuBtn(building, "btnBuild", "Images/Character/Building/" + building);
                }
            }
            ShowCancelMenu();
            return;
        }
        else if (IsHighTechBuildSelected)
        {
            foreach (var building in unitList[0].GetComponent<Worker>().HighTechBuildingList)
            {
                Transform btn = Btns.Find("btnHTBuild" + building);
                if (btn) btn.gameObject.SetActive(true);
                else
                {
                    CreateMenuBtn(building, "btnBuild", "Images/Character/Building/" + building);
                }
            }
            ShowCancelMenu();
            return;
        }
        if (unitList == null || unitList.Count == 0)
        {
            Btns.gameObject.SetActive(false);
            return;
        }
        bool isAllBattleUnit = true;
        foreach (var unit in unitList)
        {
            if (unit.UnitType != UnitTypes.Warrior && unit.UnitType != UnitTypes.Archer)
            {
                isAllBattleUnit = false;
                break;
            }
        }
        Btns.Find("btnPatrol").gameObject.SetActive(isAllBattleUnit);
        Btns.Find("btnHold").gameObject.SetActive(isAllBattleUnit);
        Btns.gameObject.SetActive(true);

        bool isAllWorker = true;
        foreach (var unit in unitList)
        {
            if (unit.UnitType != UnitTypes.Worker)
            {
                isAllWorker = false;
                break;
            }
        }
        Btns.Find("btnBuild").gameObject.SetActive(isAllWorker);
        Btns.Find("btnHTBuild").gameObject.SetActive(isAllWorker);

        bool isAllMoveable = true;
        foreach (var unit in unitList)
        {
            if (unit.UnitType != UnitTypes.Worker && unit.UnitType != UnitTypes.Warrior && unit.UnitType != UnitTypes.Archer)
            {
                isAllMoveable = false;
                break;
            }
        }
        Btns.Find("btnMove").gameObject.SetActive(isAllMoveable);
        Btns.Find("btnStop").gameObject.SetActive(isAllMoveable);

        bool isAllAtackable = true;
        foreach (var unit in unitList)
        {
            if (!(unit.CanAttackGround || unit.CanAttackAir))
            {
                isAllAtackable = false;
                break;
            }
        }
        Btns.Find("btnAttack").gameObject.SetActive(isAllAtackable);
    }
    public void OnBuildClick()
    {
        IsBuildSelected = true;
        UpdateMenuBox(_gameScript.SelectedList);
    }
    public void OnHTBuildClick()
    {
        IsHighTechBuildSelected = true;
        UpdateMenuBox(_gameScript.SelectedList);
    }
    public void ShowCancelMenu()
    {
        int count = 0;
        foreach (Transform btn in Btns)
        {
            if (btn.gameObject.activeSelf)
            {
                print($"btn active : {btn.name}");
                count++;
            }
        }
        int index = 0;
        for (int i = count; i < 5; i++)
        {
            Transform btn = Btns.Find("empty" + index);
            btn.gameObject.SetActive(true);
            btn.SetAsLastSibling();
            index++;
        }
        Transform btnCancel = Btns.Find("btnCancel");
        btnCancel.gameObject.SetActive(true);
        btnCancel.SetAsLastSibling();
    }
    public void OnCancelMenuClick()
    {
        IsBuildSelected = false;
        IsHighTechBuildSelected = false;
        UpdateMenuBox(_gameScript.SelectedList);
    }
    public void CreateMenuBtn(UnitTypes spawningUnit, string btnName, string imgPath)
    {
        Transform btn = Instantiate(Resources.Load<GameObject>("Prefab/UI/" + btnName), Btns).transform;
        print("CreateMenuBtn: " + btnName + spawningUnit);
        btn.name = btnName + spawningUnit;
        BtnMenu btnMenu = btn.GetComponent<BtnMenu>();
        btnMenu.SpawnUnit = spawningUnit;
        btnMenu.CheckConditions();
        Image img = btn.Find("Image").GetComponent<Image>();
        if (img) img.sprite = Resources.Load<Sprite>(imgPath);
        img.SetNativeSize();
        float targetWidth = 56;
        float targetHeight = 56;
        float ratio = targetWidth / targetHeight;
        float width = img.rectTransform.sizeDelta.x;
        float height = img.rectTransform.sizeDelta.y;
        if (width / height > ratio)
        {
            img.rectTransform.sizeDelta = new Vector2(targetWidth, targetWidth / ratio);
        }
        else
        {
            img.rectTransform.sizeDelta = new Vector2(targetHeight * ratio, targetHeight);
        }
    }
    public void OnCancelSpawning(int index)
    {

    }

    public void UpdateBtnMenu()
    {
        foreach (Transform btn in Btns)
        {
            BtnMenu btnMenu = btn.GetComponent<BtnMenu>();
            if (btnMenu) btnMenu.CheckConditions();
        }
    }

    public void UpdateMiniMapRectSize()
    {
        float width = _gameScript.MapBoundsMax.x - _gameScript.MapBoundsMin.x;
        float height = _gameScript.MapBoundsMax.y - _gameScript.MapBoundsMin.y;
        Vector2 uiSize = MiniMap.Find("RawImage").GetComponent<RectTransform>().sizeDelta;
        float uiWidth = uiSize.x;
        float uiHeight = uiSize.y;

        float ratio = uiWidth / width;
        if (width < height) ratio = uiHeight / height;

        Camera cam = _gameScript.TheCamera;
        MiniMap.Find("imgFrame").GetComponent<RectTransform>().sizeDelta =
             new Vector2(cam.orthographicSize * 2f * cam.aspect * ratio, cam.orthographicSize * 2f * ratio);

    }

    // Update is called once per frame
    void Update()
    {
        if (_updateCurrentRequested)
        {
            _updateCurrentRequested = false;
            _topBar.Find("Currency0/Text").GetComponent<UINumberRaiser>().SetNumber(_gameScript.Lumber);
            _topBar.Find("Currency1/Text").GetComponent<UINumberRaiser>().SetNumber(_gameScript.Gold);
        }



    }
    public void UpdateMiniMapRectPosition()
    {
        Vector2 uiSize = MiniMap.Find("RawImage").GetComponent<RectTransform>().sizeDelta;
        float uiWidth = uiSize.x;
        float uiHeight = uiSize.y;
        float width = _gameScript.MapBoundsMax.x - _gameScript.MapBoundsMin.x;
        float height = _gameScript.MapBoundsMax.y - _gameScript.MapBoundsMin.y;
        float ratio = width / uiWidth;
        if (width < height) ratio = height / uiHeight;
        float camX = _gameScript.TheCamera.transform.position.x;
        float camY = _gameScript.TheCamera.transform.position.y;

        MiniMap.Find("imgFrame").localPosition = new Vector3((camX - width / 2) / ratio, (camY - height / 2) / ratio, 0);
    }

    public void OnCommandClick(int index)
    {
        if (index == 0) _gameScript.OnMoveClick();
        else if (index == 1) _gameScript.OnStopClick();
        else if (index == 2) _gameScript.OnAttackClick();
        else if (index == 3) _gameScript.OnPatrolClick();
        else if (index == 4) _gameScript.OnHoldClick();
    }
    public void OnPointerDown(BaseEventData eventData)
    {
        print("OnPointerDown");
        Vector3 mousePosition = eventData.currentInputModule.input.mousePosition;
        MoveToMiniMap(mousePosition);
    }
    public void OnPointerDrag(BaseEventData eventData)
    {
        print("OnPointerDrag");
        Vector3 mousePosition = eventData.currentInputModule.input.mousePosition;
        MoveToMiniMap(mousePosition);
    }
    public void MoveToMiniMap(Vector3 mousePosition)
    {
        Vector3 miniMapPosition = MiniMap.position;
        Vector3 framePos = mousePosition - miniMapPosition;
        Vector2 uiSize = MiniMap.Find("RawImage").GetComponent<RectTransform>().sizeDelta;
        float uiWidth = uiSize.x;
        float uiHeight = uiSize.y;

        float width = _gameScript.MapBoundsMax.x - _gameScript.MapBoundsMin.x;
        float height = _gameScript.MapBoundsMax.y - _gameScript.MapBoundsMin.y;
        float ratio = width / uiWidth;

        Vector2 frameSize = MiniMap.Find("imgFrame").GetComponent<RectTransform>().sizeDelta;
        float frameWidth = frameSize.x;
        float frameHeight = frameSize.y;

        if (width >= height)
        {
            framePos.x = Mathf.Min(framePos.x, uiWidth / 2 - frameWidth / 2);
            framePos.x = Mathf.Max(framePos.x, -uiWidth / 2 + frameWidth / 2);

            framePos.y = Mathf.Min(framePos.y, (uiHeight * height / width) / 2 - frameHeight / 2);
            framePos.y = Mathf.Max(framePos.y, -(uiHeight * height / width) / 2 + frameHeight / 2);
        }
        else
        {
            framePos.x = Mathf.Min(framePos.x, (uiWidth * width / height) / 2 - frameWidth / 2);
            framePos.x = Mathf.Max(framePos.x, -(uiWidth * width / height) / 2 + frameWidth / 2);

            framePos.y = Mathf.Min(framePos.y, uiHeight / 2 - frameHeight / 2);
            framePos.y = Mathf.Max(framePos.y, -uiHeight / 2 + frameHeight / 2);
            ratio = height / uiHeight;
        }
        MiniMap.Find("imgFrame").localPosition = framePos;

        Vector3 cameraPosition = _gameScript.TheCamera.transform.position;
        cameraPosition.x = (_gameScript.MapBoundsMin.x + _gameScript.MapBoundsMax.x) / 2 + framePos.x * ratio;
        cameraPosition.y = (_gameScript.MapBoundsMin.y + _gameScript.MapBoundsMax.y) / 2 + framePos.y * ratio;
        MoveCameraToPosition(cameraPosition);
    }
    public bool MoveCameraToPosition(Vector3 cameraPosition)
    {
        Vector3 newPos = cameraPosition;
        newPos.x = Mathf.Clamp(newPos.x, _gameScript.MapBoundsMin.x + _gameScript.TheCamera.orthographicSize * _gameScript.TheCamera.aspect, _gameScript.MapBoundsMax.x - _gameScript.TheCamera.orthographicSize * _gameScript.TheCamera.aspect);
        newPos.y = Mathf.Clamp(newPos.y, _gameScript.MapBoundsMin.y + _gameScript.TheCamera.orthographicSize, _gameScript.MapBoundsMax.y - _gameScript.TheCamera.orthographicSize);

        bool isXModified = Mathf.Abs(newPos.x - cameraPosition.x) > 0.01f;
        bool isYModified = Mathf.Abs(newPos.y - cameraPosition.y) > 0.01f;

        _gameScript.TheCamera.transform.position = newPos;

        return isXModified && isYModified;
    }
    public void SetKeyDown(Commands command)
    {
        if (command == Commands.Move)
        {
            Btns.Find("btnMove").GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }
        else if (command == Commands.Attack)
        {
            Btns.Find("btnAttack").GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }
        else if (command == Commands.Patrol)
        {
            Btns.Find("btnPatrol").GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }
        else if (command == Commands.Hold)
        {
            Btns.Find("btnHold").GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }
        else if (command == Commands.Stop)
        {
            Btns.Find("btnStop").GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }
    }
    public void ResetKeyDown()
    {
        foreach (Transform btn in Btns)
        {
            btn.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }
    public Transform GetTopCanvas()
    {
        if (!TopCanvas) TopCanvas = Instantiate(Resources.Load<GameObject>("Prefab/UI/TopCanvas")).transform;
        return TopCanvas;
    }
    public void ShowInstantMessage(string text)
    {
        // print("ShowInstanceMessage: " + text);
        List<Transform> removeList = new List<Transform>();
        foreach (var item in _previousMessages)
        {
            if (!item) removeList.Add(item);
        }
        foreach (var item in removeList)
        {
            _previousMessages.Remove(item);
        }
        if (text.Equals(_lastMsg) && _previousMessages.Count > 0) return;
        _lastMsg = text;
        string value = LanguageManager.GetText(text);
        string messageName = "instantMessage";

        //if (_previousMessage && _previousMessage.GetComponent<InstantMessageUI>().Text.text.Equals(value)) return;

        float moveY = 50;
        foreach (var item in _previousMessages)
        {
            InstantMessageUI msg = item.GetComponent<InstantMessageUI>();
            msg.StartY += moveY;
            msg.ShowY += moveY;
            item.localPosition += new Vector3(0, moveY);
        }
        Transform pnl = Instantiate(Resources.Load<GameObject>("Prefab/UI/instantMessage"), GetTopCanvas()).transform;
        pnl.name = messageName;
        pnl.gameObject.SetActive(true);
        pnl.GetComponent<InstantMessageUI>().ShowMessage(value);
        _previousMessages.Add(pnl);
        //print("instance message: " + value);
    }
    public void CloseDialog()
    {
        Transform pnl = PopupManager.Instance.Find("pnlDialog");
        if (!pnl) pnl = PopupManager.Instance.Find("pnlBuyDialog");
        if (!pnl) pnl = PopupManager.Instance.Find("pnlCancelDialog");
        if (!pnl) pnl = PopupManager.Instance.Find("pnlNotice");
        if (!pnl) pnl = PopupManager.Instance.Find("pnlNoticeWithScroll");
        if (!pnl) pnl = PopupManager.Instance.Find("pnlMaint");
        if (pnl)
        {
            PopupBase popupBase = pnl.GetComponent<PopupBase>();
            if (popupBase != null) popupBase.ClosePopup();
            else Destroy(pnl.gameObject);
        }
    }

    public void ShowOkDialog(string descKey, UnityAction ok)
    {
        print("show ok dialog");
        descKey = LanguageManager.GetText(descKey);
        //Transform pnl = Instantiate(Resources.Load<GameObject>("Prefab/UI/Popup/pnlDialog"), PopupManager.Instance.GetNextParent()).transform;
        //pnl.GetComponent<PopupBase>().OpenPopup();
        Transform pnl = PopupManager.Instance.OpenPopupAndGet("pnlDialog").transform;
        pnl.name = "pnlDialog";
        Transform content = pnl.Find("Content");
        content.Find("Text").GetComponent<TextMeshProUGUI>().text = descKey;
        Button btn = content.Find("btnOk").GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(CloseDialog);
        btn.onClick.AddListener(ok);
        if (btn) btn.transform.localPosition = new Vector3(0, -122);
        content.Find("btnCancel").gameObject.SetActive(false);
        // Transform btnClose = pnl.Find("btnClose");
        // if (btnClose) btnClose.gameObject.SetActive(false);
        // else btnClose = content.Find("btnClose");
        // if (btnClose) btnClose.gameObject.SetActive(false);

        // 백판 클릭해서 창이 안닫힌다는 지라 이슈때문에 주석처리.
        //pnl.Find("btnBlack").GetComponent<Button>().enabled = false;
    }


    public void ShowRewardReceived(params string[] args)
    {
        //Transform pnl = Instantiate(Resources.Load<GameObject>("Prefab/UI/Popup/pnlRewardReceived"), PopupManager.Instance.GetNextParent()).transform;

        //pnl.GetComponent<PopupBase>().OpenPopup();
        StartCoroutine(ShowRewardsLater(args));
    }
    IEnumerator ShowRewardsLater(params string[] args)
    {
        Transform pnl = PopupManager.Instance.OpenPopupAndGet("pnlRewardReceived").transform;
        pnl.name = "pnlRewardReceived";
        // GameManager.Instance.ExecuteCallback(0.02f, null, () =>
        // {
        //     pnl.Find("Content").GetComponent<VerticalLayoutGroup>().enabled = true;
        // });
        //Transform pnl = TheCanvas.Find("pnlRewardReceived");
        Transform layout = pnl.Find("Content").Find("btns");
        PoolItemRecycle recycle = PoolItemRecycle.Recycle(layout);
        Dictionary<string, Transform> dic = new Dictionary<string, Transform>();
        layout.Find("temp").gameObject.SetActive(false);
        PlayerData pData = SaveData.Instance.Data;
        PopupBase popup = pnl.GetComponent<PopupBase>();
        popup.BlockClose = true;
        //Dictionary<string, object> Logdic = new Dictionary<string, object>();
        //Dictionary<string, object> temp = new Dictionary<string, object>();

        bool isLogAlready = false;


        for (int i = 0; i < args.Length; i += 2)
        {
            if (!pnl) break;
            Transform slot;
            //TEManager.Instance.TrackEvent("item_get", new Dictionary<string, object> { { "resource_info_item_id", args[i] }, { "resource_info_item_num", args[i + 1] }, { "resource_info_after_num", pData.Get(args[i]) } });
            string key = args[i].ToString();

            if (dic.ContainsKey(args[i]))
            {
                slot = dic[args[i]];
                Text lbl = slot.Find("lblCount").GetComponent<Text>();
                if (!int.TryParse(lbl.text, out var previousAmount) ||
                    !int.TryParse(args[i + 1], out var addAmount) ||
                    (long)(previousAmount + addAmount) > int.MaxValue)
                {
                    double doubleAddAmount;
                    var doublePreviousAmount = doubleAddAmount = 0;
                    if (!long.TryParse(lbl.text, out var longPreviousAmount))
                        double.TryParse(lbl.text, out doublePreviousAmount);
                    if (!long.TryParse(args[i + 1], out var longAddAmount))
                        double.TryParse(args[i + 1], out doubleAddAmount);
                    lbl.text = ((longPreviousAmount > 0 ? longPreviousAmount : doublePreviousAmount) +
                                (longAddAmount > 0 ? longAddAmount : doubleAddAmount)).ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                    lbl.text = (previousAmount + addAmount).ToString();
            }
            else
            {
                slot = recycle.GetItem(layout.Find("temp"));
                slot.name = args[i];
                // print($"slot name: {slot.name}/args: {args[i]}");
                slot.gameObject.SetActive(true);
                Image img = slot.Find("imgIcon").GetComponent<Image>();
                int rarity = 0;
                if (args[i].Equals("badge")) img.sprite = GameManager.Instance.GetSpriteFromMultiple("Images/UI/trainBug/badges", "badges_10");
                else if (args[i].Equals("upstone")) img.sprite = Resources.Load<Sprite>("Images/UI/cristal_p_upstone");
                else if (args[i].Equals("exp")) img.sprite = Resources.Load<Sprite>("Images/UI/exp");

                slot.Find("lblCount").GetComponent<Text>().text = args[i + 1];
                // print("rarity: " + rarity);
                slot.transform.Find("imgSlot").GetComponent<Image>().sprite = GameManager.Instance.GetLongSlotBackGround(rarity);
                HandleInventoryCardColor(slot.transform.Find("imgSlot"), args[i]);
                int goodTierLimit = 8;
                slot.Find("imgLight").gameObject.SetActive(rarity >= goodTierLimit);

                dic.Add(args[i], slot);
                yield return new WaitForSeconds(0.1f);
                popup.BlockClose = false;
            }
        }

    }
    public static void HandleInventoryCardColor(Transform card, string id)
    {
        int index;
        Image img = card.GetComponent<Image>();
        // print("id: " + id);
        if (int.TryParse(id.Substring(2), out index))
        {
            if (index >= 151 && index <= 158) img.sprite = GameManager.Instance.GetLongSlotBackGround(index - 151);
            else if (index >= 159 && index <= 166) img.sprite = GameManager.Instance.GetLongSlotBackGround(index - 159);
        }
    }
    public void ShowDialog(string descKey, UnityAction ok)
    {
        ShowDialog("notification", descKey, ok);
    }
    public void ShowDialog(string titleKey, string descKey, UnityAction ok)
    {
        print("show dialog: " + descKey);
        descKey = LanguageManager.GetText(descKey);
        //GameObject obj = Instantiate(Resources.Load<GameObject>("Prefab/UI/Popup/pnlDialog"), PopupManager.Instance.GetNextParent());
        GameObject obj = PopupManager.Instance.OpenPopupAndGet("pnlDialog");
        Transform pnl = obj.transform;
        pnl.name = "pnlDialog";
        //pnl.GetComponent<PopupBase>().OpenPopup();
        Transform content = pnl.Find("Content");
        content.Find("Text").GetComponent<TextMeshProUGUI>().text = descKey;
        content.Find("lblTitle").GetComponent<Localizer>().LocalizingKey = titleKey;
        Button btnOk = content.Find("btnOk").GetComponent<Button>();
        btnOk.onClick.RemoveAllListeners();
        btnOk.onClick.AddListener(CloseDialog);
        if (ok != null) btnOk.onClick.AddListener(ok);

        Button btnCancel = content.Find("btnCancel").GetComponent<Button>();
        btnCancel.onClick.RemoveAllListeners();
        btnCancel.onClick.AddListener(CloseDialog);
    }
    public void ShowCancelDialog(string descKey, UnityAction ok)
    {
        ShowCancelDialog("notification", descKey, ok);
    }
    public void ShowCancelDialog(string titleKey, string descKey, UnityAction ok)
    {
        descKey = LanguageManager.GetText(descKey);
        //GameObject obj = Instantiate(Resources.Load<GameObject>("Prefab/UI/Popup/pnlDialog"), PopupManager.Instance.GetNextParent());
        GameObject obj = PopupManager.Instance.OpenPopupAndGet("pnlCancelDialog");
        Transform pnl = obj.transform;
        pnl.name = "pnlCancelDialog";
        //pnl.GetComponent<PopupBase>().OpenPopup();
        Transform content = pnl.Find("Content");
        content.Find("Text").GetComponent<TextMeshProUGUI>().text = descKey;
        content.Find("lblTitle").GetComponent<Localizer>().LocalizingKey = titleKey;
        Button btnOk = content.Find("btnOk").GetComponent<Button>();
        btnOk.onClick.RemoveAllListeners();
        btnOk.onClick.AddListener(CloseDialog);
        if (ok != null) btnOk.onClick.AddListener(ok);

        Button btnCancel = content.Find("btnCancel").GetComponent<Button>();
        btnCancel.onClick.RemoveAllListeners();
        btnCancel.onClick.AddListener(CloseDialog);
    }
    public void ShowItemDetail(string itemID, string count)
    {
        // pnlItemDetail pnlItemDetail = PopupManager.Instance.OpenPopupAndGet("pnlItemDetail").GetComponent<pnlItemDetail>();
        // pnlItemDetail.ShowDialog(itemID);
        // Transform pnl = pnlItemDetail.transform;
        // Transform content = pnl.Find("Content");
        // content.Find("imgIcon").GetComponent<Image>().sprite = GameManager.Instance.GetSprite(itemID);

        // if (pnlPost.IsCostume(itemID))
        // {
        //     CostumeData data = DataManager.Instance.GetCostumeData(itemID);
        //     content.Find("imgIconBack").GetComponent<Image>().sprite = GameManager.Instance.GetLongSlotBackGround(data.tier - 1);
        // }

        // Text lblTitle = content.Find("lblTitle").GetComponent<Text>();

        // string strKey = itemID;
        // if (strKey.Equals("Images/UI/Shop/removeAds")) strKey = "pkg.adremove";
        // string descKey = strKey + " desc";
        // print("desc key: " + descKey);
        // if (strKey.StartsWith("i_"))
        // {
        //     InventoryData iData = DataManager.Instance.GetInventoryData(strKey);
        //     descKey = iData.ID + " desc";
        // }



        // if (string.IsNullOrEmpty(descKey)) descKey = strKey + " desc";
        // lblTitle.text = LanguageManager.GetText(strKey);

        // Text lblDesc = content.Find("lblDesc").GetComponent<Text>();

        // string strText = LanguageManager.GetText(descKey);
        // lblDesc.text = strText;
        // lblDesc.gameObject.SetActive(!strText.Equals(descKey));
        // content.Find("lblCount").GetComponent<Text>().text = count;

        // //print($"show item detail: {strKey}/{count}");

        // Button btn = content.Find("btnUse").GetComponent<Button>();
        // btn.gameObject.SetActive(false);
        // content.Find("btnOk").gameObject.SetActive(true);
        // content.Find("btnCancel").gameObject.SetActive(false);
    }
    public void ShowIndicator()
    {
        // Transform pnl = GetTopCanvas().Find("pnlIndicator");
        // if (pnl) return;

        Transform pnl = Instantiate(Resources.Load<GameObject>("Prefab/UI/Popup/pnlIndicator"), GetTopCanvas()).transform;
        pnl.name = "pnlIndicator";
        pnl.gameObject.SetActive(true);
        //print("show pnlIndicator: " + PopupManager.Instance.Find("pnlIndicator"));
    }
    public void ShowIndicator(float closeLater)
    {
        ShowIndicator();
        StartCoroutine(CloseIndicatorLater(closeLater));
    }
    IEnumerator CloseIndicatorLater(float closeLater)
    {
        yield return new WaitForSeconds(closeLater);
        HideIndicator();
    }
    public void HideIndicator()
    {
        //print("hide pnlIndicator: " + TheCanvas.transform.Find("pnlIndicator"));
        Transform pnl = GetTopCanvas().Find("pnlIndicator");
        if (pnl) Destroy(pnl.gameObject);
    }
    public void UpdateCurrency()
    {
        _updateCurrentRequested = true;
    }
}
