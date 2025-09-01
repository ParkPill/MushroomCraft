using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Localizer : MonoBehaviour
{
    public string LocalizingKey = string.Empty;
    private Text lbl;
    private TextMeshPro lblTMP;
    private TextMeshProUGUI lblTMPUGUI;
    private Image img;
    public bool Bold = true;
    public string PostFix = "";
    public string PreFix = "";
    public bool UseOutline = false;
    public float OutlineWidth = 0.252f;
    public Color OutlineColor = Color.black;
    public bool UseDilate = false;
    public float Dilate = 0.3f;
    public bool KoreanEnglishOnly = false;
    public string FirstParameter;
    // Use this for initialization
    private void Awake()
    {
        lbl = GetComponent<Text>();
        lblTMP = GetComponent<TextMeshPro>();
        lblTMPUGUI = GetComponent<TextMeshProUGUI>();
        img = GetComponent<Image>();
    }
    void Start()
    {
        UpdateLanguage();
        LanguageManager.Instance.LanguageChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged(object sender, System.EventArgs e)
    {
        UpdateLanguage();
    }
    public void SetText(string text)
    {
        LocalizingKey = text;
        UpdateLanguage();
    }

    public void UpdateLanguage()
    {
        if (img != null)
        {
            // Debug.Log(LocalizingKey);
            // print("Image set1");
            img.sprite = Resources.Load<Sprite>(LanguageManager.GetText(LocalizingKey));
            // print("Image set2");
            //img.SetNativeSize();
        }
        if (lbl != null)
        {
            if (Bold)
            {
                lbl.font = LanguageManager.Instance.GetBoldFont(KoreanEnglishOnly);
            }
            else
            {
                lbl.font = LanguageManager.Instance.GetFont(KoreanEnglishOnly);

            }
            //print("font: " + lbl.font);
            if (!string.IsNullOrEmpty(LocalizingKey))
            {
                var text = LanguageManager.GetText(LocalizingKey);
                if (text.Contains("{0}") && !string.IsNullOrEmpty(FirstParameter))
                    text = string.Format(text, FirstParameter);
                lbl.text = PreFix + text + PostFix;
                if (GetComponent<TextExposure>() != null)
                {
                    GetComponent<TextExposure>().StartTextRole(lbl.text, OnTextRoleDone);
                }
            }
        }

        if (lblTMP != null)
        {
            if (lblTMP.font.name.Contains("DNF") && LanguageManager.Instance.GetCurrentLanguage() == SystemLanguage.Korean)
            {

            }
            else
            {
                if (Bold) lblTMP.font = LanguageManager.Instance.GetTMPBoldFont(KoreanEnglishOnly);//lblTMP.fontStyle = FontStyles.Bold;
                else lblTMP.font = LanguageManager.Instance.GetTMPFont(KoreanEnglishOnly);
            }
            if (!string.IsNullOrEmpty(LocalizingKey))
            {
                var text = LanguageManager.GetText(LocalizingKey);
                if (text.Contains("{0}") && !string.IsNullOrEmpty(FirstParameter))
                    text = string.Format(text, FirstParameter);
                lblTMP.text = PreFix + text + PostFix;
                if (GetComponent<TextExposure>() != null)
                {
                    GetComponent<TextExposure>().StartTextRole(lblTMP.text, OnTextRoleDone);
                }
            }
            if (UseOutline)
            {
                lblTMP.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, OutlineWidth); // Use material instance to avoid GC
                lblTMP.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, OutlineColor); // Use material instance to avoid GC
            }
            if (UseDilate) lblTMP.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, Dilate);
        }

        if (lblTMPUGUI != null)
        {
            if (lblTMPUGUI.font.name.Contains("DNF") && LanguageManager.Instance.GetCurrentLanguage() == SystemLanguage.Korean)
            {

            }
            else
            {
                if (Bold) lblTMPUGUI.font = LanguageManager.Instance.GetTMPBoldFont(KoreanEnglishOnly);//lblTMP.fontStyle = FontStyles.Bold;
                else lblTMPUGUI.font = LanguageManager.Instance.GetTMPFont(KoreanEnglishOnly);
            }

            //lblTMPUGUI.font = LanguageManager.Instance.GetTMPFont();// LanguageManager.Instance.GetTMPUGUIFont();
            if (!string.IsNullOrEmpty(LocalizingKey))
            {
                var text = LanguageManager.GetText(LocalizingKey);
                if (text.Contains("{0}") && !string.IsNullOrEmpty(FirstParameter))
                    text = string.Format(text, FirstParameter);
                lblTMPUGUI.text = PreFix + text + PostFix;
                if (GetComponent<TextExposure>() != null)
                {
                    GetComponent<TextExposure>().StartTextRole(lblTMPUGUI.text, OnTextRoleDone);
                }
            }
            //if (UseOutline)
            {
                lblTMPUGUI.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, OutlineWidth); // Use material instance to avoid GC
                lblTMPUGUI.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, OutlineColor); // Use material instance to avoid GC   
            }
            if (UseDilate) lblTMPUGUI.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, Dilate);

        }
    }
    public void OnTextRoleDone()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
}
