using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StudioNAP
{
    public enum AvailableLanauge
    {
        English,
        Korean
    }
    public class LanguageManager
    {
        readonly string _keyLanguage = "language";
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
            SystemLanguage.Hindi,
            SystemLanguage.German,
        };

            Debug.Log("LanguageManager Init");

            _instance._currentLanguage = _instance.GetCurrentLanguage();


            UpdateFont();
            //_instance.ftKorean = Resources.Load<Font>("Font/NEXON Lv1 Gothic OTF");
            //_instance.ftEnglish = Resources.Load<Font>("Font/NEXON Lv1 Gothic OTF");
            //_instance.ftJapanese = Resources.Load<Font>("Font/NotoSansCJKjp-Bold");
            //      _instance.ftRussian = Resources.Load<Font>("Font/NotoSans-russian-Regular");

            //      _instance.ftKoreanBold = Resources.Load<Font>("Font/NEXON Lv1 Gothic OTF Bold");
            //_instance.ftEnglishBold = Resources.Load<Font>("Font/NEXON Lv1 Gothic OTF Bold");
            //_instance.ftJapaneseBold = Resources.Load<Font>("Font/NotoSansCJKjp-Bold");
            //      _instance.ftRussianBold = Resources.Load<Font>("Font/NotoSans-russian-Bold");

            //      _instance.ftTMPKorean = Resources.Load<TMP_FontAsset>("Font/NexonWhite");
            //_instance.ftTMPEnglish = Resources.Load<TMP_FontAsset>("Font/Noto");
            //_instance.ftTMPJapanese = Resources.Load<TMP_FontAsset>("Font/Noto");
            //      _instance.ftTMPRussian = Resources.Load<TMP_FontAsset>("Font/NotoSans-russian-Regular SDF");

            //      _instance.ftTMPBoldKorean = Resources.Load<TMP_FontAsset>("Font/NEXONBoldWhite");
            //      _instance.ftTMPBoldEnglish = Resources.Load<TMP_FontAsset>("Font/NEXONBoldWhite");
            //      _instance.ftTMPBoldJapanese = Resources.Load<TMP_FontAsset>("Font/NotoBold");
            //      _instance.ftTMPBoldRussian = Resources.Load<TMP_FontAsset>("Font/NotoSans-russian-Bold SDF");

            //_instance.ftTMPUGUIKorean = Resources.Load<TMP_FontAsset>("Font/NEXONBoldWhite");
            //_instance.ftTMPUGUIEnglish = Resources.Load<TMP_FontAsset>("Font/NEXONBoldWhite");
            //_instance.LoadCSV();
            _instance.LoadTSV();
        }
        #endregion

        public static string GetLanguageCode(SystemLanguage lang) => lang switch
        {
            SystemLanguage.Korean => "ko",
            SystemLanguage.English => "en",
            SystemLanguage.Japanese => "ja",
            SystemLanguage.ChineseTraditional => "zh-TW",
            SystemLanguage.German => "de",
            SystemLanguage.Vietnamese => "vi",
            SystemLanguage.Indonesian => "id",
            SystemLanguage.Thai => "th",
            SystemLanguage.Spanish => "es",
            SystemLanguage.French => "fr",
            SystemLanguage.Portuguese => "pt",
            SystemLanguage.Italian => "it",
            SystemLanguage.Dutch => "nl",
            SystemLanguage.Hindi => "hi",
            _ => "en"
        };

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
            "English"
        };
            }
            return _languageNames;
        }
        public Font LoadNormalFont(SystemLanguage language)
        {
            string path = "Noto";
            if (language == SystemLanguage.Korean)
            {
                path = "Roboto-VariableFont_wdth,wght";
                FontNormalKR = Resources.Load<Font>("Font/" + path);
            }
            else if (language == SystemLanguage.English)
            {
                path = "Roboto-VariableFont_wdth,wght";
                FontNormalEN = Resources.Load<Font>("Font/" + path);
            }
            else path = "Roboto-VariableFont_wdth,wght";
            Font font = Resources.Load<Font>("Font/" + path);
            return font;
        }
        public static void UpdateFont()
        {
            string path = "Noto";
            if (_instance._currentLanguage == SystemLanguage.Korean)
            {
                FontNormalKR = Resources.Load<Font>("Font/NEXON Lv1 Gothic OTF");
            }
            else if (_instance._currentLanguage == SystemLanguage.English)
            {
                path = "Roboto-VariableFont_wdth,wght";
                FontNormalEN = Resources.Load<Font>("Font/" + path);
            }
            else path = "Roboto-VariableFont_wdth,wght";
            FontNormal = Resources.Load<Font>("Font/" + path);

            if (_instance._currentLanguage == SystemLanguage.Korean)
            {
                path = "Roboto-VariableFont_wdth,wght";
                FontBoldKR = Resources.Load<Font>("Font/" + path);
            }
            else if (_instance._currentLanguage == SystemLanguage.English)
            {
                path = "Roboto-VariableFont_wdth,wght";
                FontBoldEN = Resources.Load<Font>("Font/" + path);
            }
            else path = "Roboto-VariableFont_wdth,wght";
            FontBold = Resources.Load<Font>("Font/" + path);


            if (_instance._currentLanguage == SystemLanguage.Korean)
            {
                path = "Roboto-VariableFont_wdth,wght SDF";
                TMPNormalKR = Resources.Load<TMP_FontAsset>("Font/" + path);
            }
            else if (_instance._currentLanguage == SystemLanguage.English)
            {
                path = "Roboto-VariableFont_wdth,wght SDF";
                TMPNormalEN = Resources.Load<TMP_FontAsset>("Font/" + path);
            }
            else path = "Roboto-VariableFont_wdth,wght SDF";
            TMPNormal = Resources.Load<TMP_FontAsset>("Font/" + path);

            if (_instance._currentLanguage == SystemLanguage.Korean)
            {
                path = "Roboto-VariableFont_wdth,wght SDF";
                TMPBoldKR = Resources.Load<TMP_FontAsset>("Font/" + path);
            }
            else if (_instance._currentLanguage == SystemLanguage.English)
            {
                path = "Roboto-VariableFont_wdth,wght SDF";
                _instance.TMPBoldEN = Resources.Load<TMP_FontAsset>("Font/" + path);
            }
            else path = "Roboto-VariableFont_wdth,wght SDF";

            TMPBold = Resources.Load<TMP_FontAsset>("Font/" + path);
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
        public void SetCurrentLanguage(SystemLanguage lang)
        {
            PlayerPrefs.SetInt(_keyLanguage, (int)lang);
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
            SystemLanguage savedLanguage = (SystemLanguage)PlayerPrefs.GetInt(_keyLanguage, -1);
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
                PlayerPrefs.SetInt(_keyLanguage, (int)savedLanguage);
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
}
