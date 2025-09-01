using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum AvailableLanauge
{
    English,
    Korean
}
public class LanguageManager
{
    private string FilePath = "saki - language";
    private Dictionary<string, Dictionary<string, string>> LanguageDic = new Dictionary<string, Dictionary<string, string>>();
    private string[] _languageArray;
    public event EventHandler LanguageChanged;
    //public Dictionary<SystemLanguage, Font> FontDic = new Dictionary<SystemLanguage, Font>();
    //   public Dictionary<SystemLanguage, Font> TMPDic = new Dictionary<SystemLanguage, Font>();
    //   public Dictionary<SystemLanguage, Font> FontBoldDic = new Dictionary<SystemLanguage, Font>();
    //   public Dictionary<SystemLanguage, Font> TMPBoldDic = new Dictionary<SystemLanguage, Font>();
    public static Font FontNormal;
    public static Font FontNormalKR;
    public static Font FontNormalEN;
    public static Font FontBold;
    public static Font FontBoldKR;
    public static Font FontBoldEN;
    public static TMP_FontAsset TMPNormal;
    public static TMP_FontAsset TMPNormalKR;
    public static TMP_FontAsset TMPNormalEN;
    public static TMP_FontAsset TMPBold;
    public static TMP_FontAsset TMPBoldKR;
    public TMP_FontAsset TMPBoldEN;
    List<string> _languageNames;
    List<SystemLanguage> _languageList;

    //   public Font ftKorean;
    //public Font ftJapanese;
    //public Font ftEnglish;
    //   public Font ftRussian;

    //   public Font ftKoreanBold;
    //public Font ftJapaneseBold;
    //public Font ftEnglishBold;
    //   public Font ftRussianBold;

    //   public Font ftArial;

    //public TMP_FontAsset ftTMPEnglish;
    //public TMP_FontAsset ftTMPKorean;
    //public TMP_FontAsset ftTMPJapanese;
    //   public TMP_FontAsset ftTMPRussian;

    //   public TMP_FontAsset ftTMPBoldEnglish;
    //   public TMP_FontAsset ftTMPBoldKorean;
    //   public TMP_FontAsset ftTMPBoldJapanese;
    //   public TMP_FontAsset ftTMPBoldRussian;

    public static List<SystemLanguage> SupportingLanguages;
    SystemLanguage _currentLanguage = SystemLanguage.Afrikaans;
    //public TMP_FontAsset ftTMPUGUIKorean;
    //public TMP_FontAsset ftAssetBMDohyun;
    #region Singleton
    private static LanguageManager _instance;
    public static LanguageManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LanguageManager();
                Init();
            }

            return _instance;
        }
    }
    static void Init()
    {
        SupportingLanguages = new List<SystemLanguage>
        {
            // SystemLanguage 리스트에 언어 추가
            SystemLanguage.Korean,
            SystemLanguage.English,
            SystemLanguage.Japanese,
            SystemLanguage.ChineseTraditional,
            SystemLanguage.Vietnamese,
            SystemLanguage.Indonesian,
            SystemLanguage.Thai,
            SystemLanguage.Spanish,
            SystemLanguage.French,
            SystemLanguage.Portuguese,
            SystemLanguage.Italian,
            SystemLanguage.Dutch,
            SystemLanguage.Hindi
        };

        Debug.Log("LanguageManager Init");

        _instance._currentLanguage = _instance.GetCurrentLanguage();


        UpdateFont();
        //_instance.ftKorean = Resources.Load<Font>("Fonts/NEXON Lv1 Gothic OTF");
        //_instance.ftEnglish = Resources.Load<Font>("Fonts/NEXON Lv1 Gothic OTF");
        //_instance.ftJapanese = Resources.Load<Font>("Fonts/NotoSansCJKjp-Bold");
        //      _instance.ftRussian = Resources.Load<Font>("Fonts/NotoSans-russian-Regular");

        //      _instance.ftKoreanBold = Resources.Load<Font>("Fonts/NEXON Lv1 Gothic OTF Bold");
        //_instance.ftEnglishBold = Resources.Load<Font>("Fonts/NEXON Lv1 Gothic OTF Bold");
        //_instance.ftJapaneseBold = Resources.Load<Font>("Fonts/NotoSansCJKjp-Bold");
        //      _instance.ftRussianBold = Resources.Load<Font>("Fonts/NotoSans-russian-Bold");

        //      _instance.ftTMPKorean = Resources.Load<TMP_FontAsset>("Fonts/NexonWhite");
        //_instance.ftTMPEnglish = Resources.Load<TMP_FontAsset>("Fonts/Noto");
        //_instance.ftTMPJapanese = Resources.Load<TMP_FontAsset>("Fonts/Noto");
        //      _instance.ftTMPRussian = Resources.Load<TMP_FontAsset>("Fonts/NotoSans-russian-Regular SDF");

        //      _instance.ftTMPBoldKorean = Resources.Load<TMP_FontAsset>("Fonts/NEXONBoldWhite");
        //      _instance.ftTMPBoldEnglish = Resources.Load<TMP_FontAsset>("Fonts/NEXONBoldWhite");
        //      _instance.ftTMPBoldJapanese = Resources.Load<TMP_FontAsset>("Fonts/NotoBold");
        //      _instance.ftTMPBoldRussian = Resources.Load<TMP_FontAsset>("Fonts/NotoSans-russian-Bold SDF");

        //_instance.ftTMPUGUIKorean = Resources.Load<TMP_FontAsset>("Fonts/NEXONBoldWhite");
        //_instance.ftTMPUGUIEnglish = Resources.Load<TMP_FontAsset>("Fonts/NEXONBoldWhite");
        //_instance.LoadCSV();
        _instance.LoadTSV();
    }
    #endregion

    public string GetLanguageCode(SystemLanguage lang)
    {
        switch (lang)
        {
            case SystemLanguage.Korean:
                return "ko";
            case SystemLanguage.English:
                return "en";
            case SystemLanguage.Japanese:
                return "ja";
            case SystemLanguage.ChineseTraditional:
                return "zh-TW";
            case SystemLanguage.German:
                return "de";
            case SystemLanguage.Vietnamese:
                return "vi";
            case SystemLanguage.Indonesian:
                return "id";
            case SystemLanguage.Thai:
                return "th";
            case SystemLanguage.Spanish:
                return "es";
            case SystemLanguage.French:
                return "fr";
            case SystemLanguage.Portuguese:
                return "pt";
            case SystemLanguage.Italian:
                return "it";
            case SystemLanguage.Dutch:
                return "nl";
            case SystemLanguage.Hindi:
                return "hi";
            default:
                return "en";
        }
    }

    public string GetCurrentLanguageCode()
    {
        return GetLanguageCode(_currentLanguage);
    }
    public List<SystemLanguage> GetLanguageList()
    {
        if (_languageList == null)
        {
            _languageList = new List<SystemLanguage>
        {
            SystemLanguage.Korean,
            SystemLanguage.English,
            SystemLanguage.Japanese,
            SystemLanguage.ChineseTraditional,
            SystemLanguage.German,
            SystemLanguage.Vietnamese,
            SystemLanguage.Indonesian,
            SystemLanguage.Thai,
            SystemLanguage.Spanish,
            SystemLanguage.French,
            SystemLanguage.Portuguese,
            SystemLanguage.Italian,
            SystemLanguage.Dutch,
                SystemLanguage.Hindi
            };
        }
        return _languageList;
    }
    public List<string> GetLanguageNames()
    {
        if (_languageNames == null)
        {
            _languageNames = new List<string>
        {
            "한국어",
            "English",
            "日本語",
            "繁體中文",
            "Deutsch",
            "Tiếng Việt",
            "Bahasa Indonesia",
            "ไทย",
            "Español",
            "Français",
            "Português",
            "Italiano",
            "Nederlands",
            "हिन्दी"
        };
        }
        return _languageNames;
    }
    public static void UpdateFont()
    {
        string path = "Noto";
        if (_instance._currentLanguage == SystemLanguage.Korean)
        {
            path = "NEXON Lv1 Gothic OTF";
            FontNormalKR = Resources.Load<Font>("Fonts/" + path);
        }
        else if (_instance._currentLanguage == SystemLanguage.English)
        {
            path = "NEXON Lv1 Gothic OTF";
            FontNormalEN = Resources.Load<Font>("Fonts/" + path);
        }
        else if (_instance._currentLanguage == SystemLanguage.Japanese) path = "NotoSansCJKjp-Regular";
        else if (_instance._currentLanguage == SystemLanguage.ChineseTraditional) path = "NotoSansCJKjp-Regular";
        else if (_instance._currentLanguage == SystemLanguage.Thai) path = "NotoSansThai-Bold";
        else if (_instance._currentLanguage == SystemLanguage.Russian ||
                 _instance._currentLanguage == SystemLanguage.Hindi) path = "NotoSans-russian-Regular";
        else path = "NotoSansCJKjp-Regular";
        FontNormal = Resources.Load<Font>("Fonts/" + path);

        if (_instance._currentLanguage == SystemLanguage.Korean)
        {
            path = "NEXON Lv1 Gothic OTF Bold";
            FontBoldKR = Resources.Load<Font>("Fonts/" + path);
        }
        else if (_instance._currentLanguage == SystemLanguage.English)
        {
            path = "NEXON Lv1 Gothic OTF Bold";
            FontBoldEN = Resources.Load<Font>("Fonts/" + path);
        }
        else if (_instance._currentLanguage == SystemLanguage.Japanese) path = "NotoSansCJKjp-Bold";
        else if (_instance._currentLanguage == SystemLanguage.ChineseTraditional) path = "NotoSansCJKjp-Bold";
        else if (_instance._currentLanguage == SystemLanguage.Thai) path = "NotoSansThai-Bold";
        else if (_instance._currentLanguage == SystemLanguage.Russian ||
                    _instance._currentLanguage == SystemLanguage.Hindi) path = "NotoSans-russian-Bold";
        else path = "NotoSansCJKjp-Bold";
        FontBold = Resources.Load<Font>("Fonts/" + path);


        if (_instance._currentLanguage == SystemLanguage.Korean)
        {
            path = "NexonWhite";
            TMPNormalKR = Resources.Load<TMP_FontAsset>("Fonts/" + path);
        }
        else if (_instance._currentLanguage == SystemLanguage.English)
        {
            path = "NexonWhite";
            TMPNormalEN = Resources.Load<TMP_FontAsset>("Fonts/" + path);
        }
        else if (_instance._currentLanguage == SystemLanguage.Japanese) path = "Noto";
        else if (_instance._currentLanguage == SystemLanguage.ChineseTraditional) path = "Noto";
        else if (_instance._currentLanguage == SystemLanguage.Thai) path = "NotoSansThai-Bold SDF";
        else if (_instance._currentLanguage == SystemLanguage.Russian ||
                    _instance._currentLanguage == SystemLanguage.Hindi) path = "NotoSans-russian-Regular SDF";
        else path = "Noto";
        TMPNormal = Resources.Load<TMP_FontAsset>("Fonts/" + path);

        if (_instance._currentLanguage == SystemLanguage.Korean)
        {
            path = "NEXONBoldWhite";
            TMPBoldKR = Resources.Load<TMP_FontAsset>("Fonts/" + path);
        }
        // else if (_instance._currentLanguage == SystemLanguage.English)
        // {
        path = "NEXONBoldWhite";
        _instance.TMPBoldEN = Resources.Load<TMP_FontAsset>("Fonts/" + path);
        // }
        if (_instance._currentLanguage == SystemLanguage.Japanese) path = "NotoBold";
        else if (_instance._currentLanguage == SystemLanguage.ChineseTraditional) path = "NotoBold";
        else if (_instance._currentLanguage == SystemLanguage.Thai) path = "NotoSansThai-Bold SDF";
        else if (_instance._currentLanguage == SystemLanguage.Russian ||
                    _instance._currentLanguage == SystemLanguage.Hindi) path = "NotoSans-russian-Bold SDF";
        else path = "NotoBold";

        Debug.Log("TMP bold: " + path);
        TMPBold = Resources.Load<TMP_FontAsset>("Fonts/" + path);
    }
    public void LoadCSV()
    {
        string _alternatePrefix = "29ffkkdjel_";
        List<string> _alternateList = new List<string>();
        var ta = Resources.Load(FilePath) as TextAsset;
        if (ta == null) return;
        string wholeText = ta.text;
        int alternateCount = 0;
        if (wholeText.Contains("\""))
        {
            int inspectStartIndex = 0;
            int quoteIndex = 0;
            bool isQuoteOn = false;

            while ((quoteIndex = wholeText.IndexOf("\"", inspectStartIndex)) >= 0)
            {
                isQuoteOn = !isQuoteOn;

                inspectStartIndex = quoteIndex;

                int quoteEndIndex = wholeText.IndexOf("\"", inspectStartIndex + 1);

                string quotation = wholeText.Substring(quoteIndex, quoteEndIndex - quoteIndex);
                wholeText = wholeText.Remove(quoteIndex, quoteEndIndex - quoteIndex + 1);
                wholeText = wholeText.Insert(quoteIndex, string.Format("{0}{1}", _alternatePrefix, alternateCount));
                alternateCount++;
                _alternateList.Add(quotation);
            }
        }

        var arrayString = wholeText.Split(new string[] { "\r\n" }, StringSplitOptions.None);
        int index = 0;

        foreach (var line in arrayString)
        {
            string filteredLine = line;
            var values = filteredLine.Split(',');
            if (index == 0)
            {
                foreach (string str in values)
                {
                    if (!string.IsNullOrEmpty(str) && !LanguageDic.ContainsKey(str))
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        string key = str.Replace(System.Environment.NewLine, "");
                        LanguageDic.Add(key, dic);
                    }
                }
                _languageArray = values;
            }
            else
            {
                for (int i = 1; i < _languageArray.Length; i++)
                {
                    string str = values[i];

                    if (string.IsNullOrEmpty(_languageArray[i]))
                    {
                        continue;
                    }
                    if (!LanguageDic[_languageArray[i]].ContainsKey(values[0]))
                    {
                        LanguageDic[_languageArray[i]].Add(values[0], str);
                    }
                }
            }
            index++;
        }
    }
    public void LoadTSV()
    {
        // Debug.Log("LanguageManager LoadTSV1");
        string _alternatePrefix = "29ffkkdjel_";
        List<string> _alternateList = new List<string>();
        var ta = Resources.Load(FilePath) as TextAsset;
        if (ta == null) return;
        string wholeText = ta.text;
        int alternateCount = 0;
        // Debug.Log("LanguageManager LoadTSV2");
        // 처리: 따옴표를 특수 접두어로 대체
        if (wholeText.Contains("\""))
        {
            int inspectStartIndex = 0;
            int quoteIndex = 0;
            bool isQuoteOn = false;

            while ((quoteIndex = wholeText.IndexOf("\"", inspectStartIndex)) >= 0)
            {
                isQuoteOn = !isQuoteOn;
                inspectStartIndex = quoteIndex;
                int quoteEndIndex = wholeText.IndexOf("\"", inspectStartIndex + 1);
                string quotation = wholeText.Substring(quoteIndex, quoteEndIndex - quoteIndex);
                wholeText = wholeText.Remove(quoteIndex, quoteEndIndex - quoteIndex + 1);
                wholeText = wholeText.Insert(quoteIndex, string.Format("{0}{1}", _alternatePrefix, alternateCount));
                alternateCount++;
                _alternateList.Add(quotation);
            }
        }

        // 줄 단위로 텍스트를 분리
        var arrayString = wholeText.Split(new string[] { "\r\n" }, StringSplitOptions.None);
        int index = 0;

        foreach (var line in arrayString)
        {
            string filteredLine = line;
            // 탭으로 분리하여 각 열의 값을 가져옴
            var values = filteredLine.Split('\t');
            // print("filteredLine: " + filteredLine);
            if (index == 0)
            {
                foreach (string str in values)
                {
                    if (!string.IsNullOrEmpty(str) && !LanguageDic.ContainsKey(str))
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        string key = str.Replace(System.Environment.NewLine, "");
                        LanguageDic.Add(key, dic);
                    }
                }
                _languageArray = values;
            }
            else
            {
                for (int i = 1; i < _languageArray.Length; i++)
                {
                    string str = values[i];
                    if (string.IsNullOrEmpty(_languageArray[i]))
                    {
                        continue;
                    }
                    if (!LanguageDic[_languageArray[i]].ContainsKey(values[0]))
                    {
                        LanguageDic[_languageArray[i]].Add(values[0], str);
                    }
                }
            }
            index++;
        }
        // Debug.Log("LanguageManager LoadTSV3");
    }

    public static string GetText(string key)
    {
        if (string.IsNullOrEmpty(key)) return key;
        // Debug.Log(string.Format("get text {0}", key));

        string language = Instance._currentLanguage.ToString();
        //if (key.Contains("loading_screen_desc_")) Debug.Log("loading_screen_desc_2: " + _instance.LanguageDic[language].ContainsKey(key));
        if (_instance.LanguageDic.ContainsKey(language) && _instance.LanguageDic[language].ContainsKey(key))
        {
            string text = _instance.LanguageDic[language][key];
            return text.Replace('$', '\n');
        }
        else
        {
            return key;
        }
    }
    public bool IsChatLanguageOn(string languageCode)
    {
        foreach (var lang in GetLanguageList())
        {
            string lCode = GetLanguageCode(lang);
            if (lCode.Equals(languageCode))
            {
                int defaultLanguage = LanguageManager.Instance.GetCurrentLanguage() == lang ? 1 : 0;
                bool isOn = PlayerPrefs.GetInt(string.Format(Consts.Key_ChatLanguageOnFormat, lang), defaultLanguage) == 1;
                if (isOn)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }
    public void SetCurrentLanguage(SystemLanguage lang)
    {
        Debug.Log("langage: " + lang);
        PlayerPrefs.SetInt(Consts.KEY_LANGUAGE, (int)lang);
        _currentLanguage = lang;
        UpdateFont();
        FireChangedEvent();
    }
    public void FireChangedEvent()
    {
        LanguageChanged?.Invoke(this, new EventArgs());
    }
    public SystemLanguage GetCurrentLanguage()
    {
        // Debug.Log("LanguageManager GetCurrentLanguage");
        SystemLanguage savedLanguage = (SystemLanguage)PlayerPrefs.GetInt(Consts.KEY_LANGUAGE, -1);
        if (savedLanguage == (SystemLanguage)(-1))
        {
            SystemLanguage sysLang = Application.systemLanguage;

            if (SupportingLanguages.Contains(sysLang))
            {
                savedLanguage = sysLang;
            }
            else
            {
                // Debug.Log("LanguageManager GetCurrentLanguage2");
                switch (sysLang)
                {
                    case SystemLanguage.French:
                        savedLanguage = SystemLanguage.French;
                        break;

                    case SystemLanguage.Portuguese:
                        savedLanguage = SystemLanguage.Portuguese;
                        break;

                    case SystemLanguage.Chinese:
                    case SystemLanguage.ChineseSimplified:
                        savedLanguage = SystemLanguage.ChineseTraditional;
                        break;

                    case SystemLanguage.Spanish:
                        savedLanguage = SystemLanguage.Spanish;
                        break;

                    default:
                        savedLanguage = SystemLanguage.English;
                        break;
                }
            }
            // Debug.Log("LanguageManager GetCurrentLanguage3: " + savedLanguage);
            PlayerPrefs.SetInt(Consts.KEY_LANGUAGE, (int)savedLanguage);
            PlayerPrefs.Save();
        }

        return savedLanguage;
    }
    public Font GetFont(bool KoreanEnglishOnly = false)
    {
        if (KoreanEnglishOnly)
        {
            if (_currentLanguage == SystemLanguage.Korean) return FontNormalKR;
            else return FontNormalEN;
        }
        return FontNormal;//GetFont(GetCurrentLanguage());
    }
    public TMP_FontAsset GetTMPFont(bool KoreanEnglishOnly = false)
    {
        if (KoreanEnglishOnly)
        {
            if (_currentLanguage == SystemLanguage.Korean) return TMPNormalKR;
            else return TMPNormalEN;
        }
        return TMPNormal;//GetTMPFont(GetCurrentLanguage());
    }
    public TMP_FontAsset GetTMPBoldFont(bool KoreanEnglishOnly = false)
    {
        if (KoreanEnglishOnly)
        {
            if (_currentLanguage == SystemLanguage.Korean) return TMPBoldKR;
            else return TMPBoldEN;
        }
        return TMPBold;//GetTMPBoldFont(GetCurrentLanguage());
    }
    public Font GetBoldFont(bool KoreanEnglishOnly = false)
    {
        if (KoreanEnglishOnly)
        {
            if (_currentLanguage == SystemLanguage.Korean) return FontBoldKR;
            else return FontBoldEN;
        }
        return FontBold;//GetBoldFont(GetCurrentLanguage());
    }

    public Font GetFont(SystemLanguage lang)
    {
        return FontNormal;
    }
    public TMP_FontAsset GetTMPFont(SystemLanguage lang)
    {
        return TMPNormal;
    }
    public TMP_FontAsset GetTMPBoldFont(SystemLanguage lang)
    {
        return TMPBold;
    }
    public Font GetBoldFont(SystemLanguage lang)
    {
        return FontBold;
    }

    public static void SetText(Text lbl, string key)
    {
        Font font = Instance.GetFont();
        Localizer localizer = lbl.GetComponent<Localizer>();
        if (localizer && localizer.Bold)
        {
            font = _instance.GetBoldFont();
            localizer.LocalizingKey = key;
        }
        lbl.font = font;
        lbl.text = GetText(key);
    }
    public static void SetText(TextMeshProUGUI lbl, string key)
    {
        if (Instance == null) Debug.Log("LanguageManager null check");
        TMP_FontAsset font = TMPNormal;
        Localizer localizer = lbl.GetComponent<Localizer>();
        if (localizer && localizer.Bold) font = TMPBold;
        lbl.font = font;
        lbl.text = GetText(key);
    }
    public static Text SetText(Transform lbl, string key)
    {
        Text text = lbl.GetComponent<Text>();
        if (text != null)
            SetText(text, key);
        else
        {
            TextMeshProUGUI textUGUI = lbl.GetComponent<TextMeshProUGUI>();
            SetText(textUGUI, key);
        }
        return text;
    }
}
