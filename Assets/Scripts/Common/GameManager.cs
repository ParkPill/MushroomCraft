using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StrEventArg : EventArgs
{
    public string StringValue = string.Empty;
    public StrEventArg() { }
    public StrEventArg(string value)
    {
        StringValue = value;
    }
}
public class GameObjectEventArg : EventArgs
{
    public GameObject TheGameObject;
    public GameObjectEventArg() { }
    public GameObjectEventArg(GameObject value)
    {
        TheGameObject = value;
    }
}
public enum Stages
{
    Lobby = 0,
    Forest0
}

public struct UIAlphaElements
{
    public List<Image> images;
    public List<Text> texts;
    public List<TextMeshProUGUI> tmpros;
    public List<SpriteRenderer> renderers;
}


public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public bool IsChatTried = false;
    // [HideInInspector]
    public List<GameObject> ResourceCache = new List<GameObject>();
    bool _isPreloadedResources = false;
    public bool PingFailed = false;
    public bool HasBeenGameScene = false;
    public int Spawn_Up_1 = 10;
    public int Spawn_Up_2 = 20;
    public int Spawn_Up_3 = 20;
    public int Spawn_Up_4 = 10;
    public int Spawn_Down_1 = 5;
    public int Spawn_Down_2 = 15;
    public int Spawn_Down_3 = 15;
    public int Spawn_Down_4 = 5;
    public int SpawnDistance = 30;
    public float AttackSpeed = 1;

    // public bool DiceRankRefreshRequired = false;
    // public bool MaskRankRefreshRequired = false;
    // public bool GodGiftRankRefreshRequired = false;
    // public bool MineRankRefreshRequired = false;
    // public bool DragonRankRefreshRequired = false;
    // public bool IOGameRankRefreshRequired = false;
    // public bool ArenaRankRefreshRequired = false;
    public int SpawnCountPerOnce = 4;
    public bool IsFirstLaunch = true;
    public int NoticeIndex = 0;
    public int MaintIndex = 0;
    [HideInInspector]
    public DateTime LastGuildInfoCheckTime = DateTime.MinValue;

    public UnityAction GachaLevelChanged;
    [HideInInspector]
    public Vector2 MapLeftTop = new Vector2(-35, 37f);
    [HideInInspector]
    public Vector2 MapRightBottom = new Vector2(28, -2);
    // photon fusion 
    // dev
    // a2bb45e2-c376-48f3-8848-cbbaee9df0ae
    // live
    // 0137de63-5de2-47b5-9493-2d22033d3c30

    public static string Version;
    public static bool IsTestVersion = true; // test 
    public static bool IsOneStore = false; // test 
    public static bool isPaidVersion = true;
    //public static bool oneCheck = false; // 접속시 1번만 체크
    public static string GetBuildVersion()
    {
        string platform = string.Empty;
#if UNITY_IOS
        platform = "ios";
#else
        if (IsOneStore == true)
            platform = "one";
        else
            platform = "aos";
#endif
        var server = IsTestVersion ? "(dev)" : "(live)";

        return $"{platform} V{Version} {server}";
    }
    public string PopupToOpen;
    int _enemyDeadCount = 0;
    DateTime _huntStartCount;
    public bool IsMovingByJoystick;
    public long[] buffTimes = new long[3];
    public DateTime Today;
    public int StartDoor;
    public bool SingleMode = false;
    [HideInInspector]
    public int BossCount = 10;
    public float TenMinTimer = 0; // starts with 8 min so that something can happen in 2 min
    public float NineMinTimer = 540;
    public string TestText;
    public bool IsTitleThrough;
    public bool IsSignInSkipped = false;
    public int screenWidth;
    public int screenHeight;
    public Vector2 JoystickValue;
    public bool IsBossChallenge = false;
    public bool IsNewDay = false;
    public bool QuestLockRemove = false;
    // free to paid change list
    // AdmobManager - google ads app id
    // IAPManager - receipt
    // GPGSManager
    // Firebase google-services.json
    [HideInInspector]
    public int MaxLevel = 199;
    public bool ShootExtraHeart = false;
    [HideInInspector]
    public bool FromGuildBoss = false;
    public bool FromLegendGirl = false;
    public bool FromPVPTeam = false;
    public bool FromPVPTag = false;
    public bool FromPVP1On1 = false;
    public bool IsMaintenanceOn = false;
    public bool IsTestDamageOn = false;
    public int SeasonIndex = 0;
    [HideInInspector]

    public int RaidBossIndex = 0;
    [HideInInspector]
    public int MaxHeroCount = 21;
    [HideInInspector]
    //public int MaxSwordCount = 24;
    public int MaxShieldCount = 12;
    //public GuildInfo TheGuildInfo = null;
    public string SceneName = "";
    public GameScript TheGameScript = null;
    public Title TheTitle = null;
    public int Channel = 3; // 
    public bool IsChannelChanging = false;
    public bool IsFromTitle = false;
    public bool IsShutdownOnPurpose = false;
    //public StageTypes StageType = StageTypes.Normal;
    public static System.Random Random = new System.Random();
    public static System.Random DailyRandom = new System.Random();
    public GameObject LoadingPrefab;
    public GameObject CurrentPopup;
    public float Power = 10;
    public int SkillTestHero = 0;
    public int TutorialChecker = -1;
    public AudioSource BGMAudioSource;
    public UnityAction BlockUserChanged;
    public bool CancelRestart = false;
    public int CurrentCoinEvent = -1;
    public bool IsGodGiftEventOn = false;
    public bool IsMineEventOn = false;
    public bool IsMaskEventOn = false;
    public bool IsDiceEventOn = false;
    //public float CoinGetPossibility = 0.01f; // 1%
    [HideInInspector]
    public float LotteryGetPossibility = 0.0001f; // 0.01%
    //public PVPTypes PVPTypeToPlay = PVPTypes.PVPNone;
    //public PVPUserInfo PVPMatch;
    //public List<PVPUserInfo> PVPMatchList = new List<PVPUserInfo>();
    //public int PVPMatchIndex;
    public bool BlockServerCheck = false;
    //public GameData GameData;
    //private string _gameDataFileName = "/msfsyis.sis";
    //public event EventHandler CharacterChanged;
    //public event OnEventDelegate<VideoType> VideoComplete;
    public int CurrentStage = (int)Stages.Lobby;
    //private VideoType _playedVideoType;
    public bool IsRestoreByPlayIDRequested = false;
    //private BannerView bannerView;
    //private InterstitialAd interstitial;
    public UnityAction<string> ContentLockChanged;
    public bool IsTotemPresetChanged = false;
    public bool IsTotemPopupFromPreset = false;
    public int LegendGirlRank = 0;
    public int DungeonStage = 0;
    // public PlatformPlayer PHero;
    public string PVPAsyncData;
    public string PVPAsyncName;
    public int PVPAsyncOpponentScore = 0;
    public int MaxDungeonStage = 80;
    public bool MonsterWaveByServer = false;
    public bool MonsterWaveByTime = false;
    public int LastEnterIndex = -1;
    public UnityAction QuestUpdated;
    public bool PostExist = false;
    public bool IsSkillRangeInfinity = false;
    public bool IsOfflineStarted = true;
    public int DungeonLevel = 0;
    public bool AutoChallengeNextLevel = false;
    public bool IsAdRemoved = false;
    public List<int> SkillUpgradeCost;

    private AudioMixerGroup defaultSFXMixer;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
                if (!_instance)
                {
                    GameObject container = new GameObject();
                    _instance = container.AddComponent(typeof(GameManager)) as GameManager;
                    _instance.name = "GameManager";
                }
#if RELEASE
                IsTestVersion = false;
#else
                IsTestVersion = true;
#endif

#if ONESTORE
                IsOneStore = true;
#else
                IsOneStore = false;
#endif

                DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }
    private void Start()
    {
        //byte[] array = new byte[] { 0, 0, 0, 0};

        //int index = 1;
        //SetBit(array, index, true);
        //print("result = " + array[0]); // 2

        //array = new byte[] { 0b_0000_0010, 0 ,0,0};
        //index = 1;
        //SetBit(array, index, false);
        //print("result = " + array[0]); // 0

        //array = new byte[] { 0b_0000_0011, 0b_0000_0011, 0,0 };
        //index = 9;
        //SetBit(array, index, false);
        //print("result = " + array[1]); // 1

        //array = new byte[] { 0b_0000_0011, 0 ,0,0};
        //index = 7;
        //SetBit(array, index, true);
        //print("result = " + array[0]); // 131
        InitSound();
    }


    public void InitSound()
    {
        if (defaultSFXMixer == null)
        {
            defaultSFXMixer = Resources.Load<AudioMixerGroup>("Settings/SFX");
            if (defaultSFXMixer == null)
                Debug.LogWarning("[GameManager] 기본 SFX 믹서를 찾을 수 없습니다.");
        }
    }

    public static bool IsAttackable(UnitBase attacker, UnitBase target)
    {
        if (attacker.CanAttackGround && target.MoveOnGround) return true;
        if (attacker.CanAttackAir && target.MoveOnAir) return true;
        return false;
    }

    public void CheckAdRemoved()
    {
        // int count = SaveData.Instance.Data.GetIAPCount(Consts.IAP_PACK_REMOVE_ADS);
        // print("ads count: " + count);
        // IsAdRemoved = count > 0;
    }
    /// <summary>
    /// returns count of result of Physics2D.OverlapCircleAll
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public int IsThereSomethingCircle(Vector2 pos, float radius, params string[] layerNames)
    {
        LayerMask layer = LayerMask.GetMask(layerNames);
        return Physics2D.OverlapCircleAll(pos, radius, layer).Length;
    }
    //public bool GetBit(byte[] array, int index)
    //{
    //    if (array == null || array.Length <= index/8)
    //    {
    //        return false;
    //    }
    //    byte byteToCompare = array[index / 8];
    //    int numToCompare = 0b_0000_0001 << (index%8);
    //    return (numToCompare & byteToCompare) > 0;
    //}
    public void SetBit(byte[] array, int index, bool value)
    {
        int byteIndex = index / 8;
        byte bitNum = 0b_0000_0001;
        byte numToCompare = (byte)(bitNum << (index % 8));
        print(string.Format("set bit: {0}/{1}", index, array.Length));
        if (value)
        {
            array[byteIndex] = (byte)(array[byteIndex] | numToCompare);
        }
        else
        {
            byte allOne = 0b_1111_1111;
            numToCompare = (byte)(numToCompare ^ allOne);

            array[byteIndex] = (byte)(array[byteIndex] & numToCompare);
        }
    }
    public bool GetBit(byte[] array, int index)
    {
        if (array == null || array.Length <= index / 8)
        {
            return false;
        }
        byte byteToCompare = array[index / 8];
        int numToCompare = 0b_0000_0001 << (index % 8);
        return (numToCompare & byteToCompare) > 0;
    }
    public void ChangeLayersRecursively(Transform trans, string name)
    {
        trans.gameObject.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in trans)
        {
            ChangeLayersRecursively(child, name);
        }
    }
    static Dictionary<string, GameObject> _prefabCache = new Dictionary<string, GameObject>();
    public static GameObject InstantiatePrefab(string path, Transform parent = null)
    {
        GameObject prefabObj;
        if (_prefabCache.ContainsKey(path))
        {
            prefabObj = _prefabCache[path];
        }
        else
        {
            prefabObj = Resources.Load<GameObject>("Prefab/" + path);
            _prefabCache.Add(path, prefabObj);
        }
        return Instantiate(prefabObj, parent);
    }
    public void PreloadMajorResource()
    {
        if (_isPreloadedResources) return;
        _isPreloadedResources = true;
        List<string> prefabs = new List<string>(){
            "UI/skill_cutscene/skill_cutscene",
        };
        foreach (var prefab in prefabs)
        {
            print("preload major resource: " + prefab);
            GameObject prefabObj = Resources.Load<GameObject>(prefab);
            ResourceCache.Add(prefabObj);
            Destroy(Instantiate(prefabObj));
            // GameObject instance;
            // if (prefabObj.GetComponent<RectTransform>() != null)
            // {
            //     instance = Instantiate(prefabObj, transform.Find("Canvas"));
            // }
            // else
            // {
            //     instance = Instantiate(prefabObj, transform);
            // }

            // instance.SetActive(false);
            // instance.name = prefabObj.name;
            // InstanceCache.Add(instance);
        }
    }
    public GameObject Instantiates(string prefab, Transform parent = null)
    {
        if (ResourceCache.Find(obj => obj.name == prefab) != null)
        {
            // print("use preloaded major resource: " + prefab);
            GameObject preloadedObj = ResourceCache.Find(obj => obj.name == prefab);
            // ResourceCache.Remove(preloadedObj);
            return Instantiate(preloadedObj, parent);
        }
        GameObject prefabObj = Resources.Load<GameObject>(prefab);
        ResourceCache.Add(prefabObj);
        return Instantiate(prefabObj, parent);
    }
    // public GameObject Instantiates(GameObject prefab, Transform parent = null)
    // {
    //     // if (InstanceCache.Find(obj => obj.name == prefab.name) != null)
    //     // {
    //     //     GameObject preloadedObj = InstanceCache.Find(obj => obj.name == prefab.name);
    //     //     Vector3 localPosition = preloadedObj.transform.localPosition;
    //     //     Vector3 localScale = preloadedObj.transform.localScale;
    //     //     InstanceCache.Remove(preloadedObj);
    //     //     preloadedObj.transform.SetParent(parent);
    //         // ExecuteCallback(0.05f, null, () =>
    //         // {
    //         //     preloadedObj.transform.localPosition = localPosition;
    //         //     preloadedObj.transform.localScale = localScale;
    //         //     print("localPosition xyz2: " + localPosition.x + "/" + localPosition.y + "/" + localPosition.z);
    //         // });

    //         // preloadedObj.SetActive(true);
    //     //     return preloadedObj;
    //     // }
    //     GameObject obj = Instantiate(prefab, parent);
    //     return obj;
    // }
    public int GetMaxSkillGauge()
    {
        return 100;
    }
    public bool UseItem(string item, int count)
    {
        print("use item: " + item + "/" + count);
        return true;
    }
    public Sprite GetPetIcon(int index)
    {
        return Resources.Load<Sprite>("Images/UI/Icon/pet/pet" + (index + 1));
    }

    public Sprite GetSkillIcon(int skillIndex)
    {
        //return Resources.Load<Sprite>(string.Format("Images/UI/Icon/Skill/{0}", (Skills)index));
        //return Resources.Load<Sprite>("Images/UI/Icon/" + (skillIndex < 0 ? "icon_plus" : string.Format("Skill/Active/skill{0}", skillIndex)));
        //if (skillIndex == 3 || skillIndex == 4) skillIndex -= 2; // normal attak for Reiner and Monica
        string strPath = "Images/UI/Icon/Skill/" + string.Format("skill{0}", skillIndex);
        // print($"str skill path: {strPath}/{skillIndex}");
        return skillIndex < 0 ? null : Resources.Load<Sprite>(strPath);
    }

    public Sprite GetSkillPassiveSprite(string id)
    {
        int sptIndex = 0;
        if (id.Equals("s_str_passive")) sptIndex = 5;
        else if (id.Equals("s_int_passive")) sptIndex = 9;
        else if (id.Equals("s_str")) sptIndex = 5;
        else if (id.Equals("s_int")) sptIndex = 9;
        else if (id.Equals("s_strF")) sptIndex = 6;
        else if (id.Equals("s_intF")) sptIndex = 10;
        else if (id.Equals("s_crit") || id.Equals("s_damF")) sptIndex = 7;
        else if (id.Equals("s_critDam") || id.Equals("s_skillDam")) sptIndex = 8;
        else if (id.Equals("s_expGain")) sptIndex = 4;
        else if (id.Equals("s_loots_gold")) sptIndex = 11;
        else if (id.Equals("s_hp") || id.Equals("s_hpF")) sptIndex = 1;
        else if (id.StartsWith("s_hpGen")) sptIndex = 2;
        else if (id.Equals("s_reduceDam") || id.StartsWith("s_def")) sptIndex = 3;

        // print($"id {id}/sptIndex");
        string strPath = "Images/UI/Icon/Skill/" + string.Format("skillPassive{0}", sptIndex);
        return string.IsNullOrEmpty(id) ? null : Resources.Load<Sprite>(strPath);
    }
    public bool WasMondayMidnightBetween(DateTime lastWeeklyTime, DateTime now)
    {
        // Find the next Monday 14:00 after lastWeeklyTime
        DateTime nextMondayMidnight = GetNextMondayMidnight(lastWeeklyTime);

        // Check if that Monday 14:00 is before now
        return nextMondayMidnight <= now;
    }

    public DateTime GetNextMondayMidnight(DateTime fromTime)
    {
        // Find the next Monday after fromTime
        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)fromTime.DayOfWeek + 7) % 7;
        if (daysUntilMonday == 0 && fromTime.TimeOfDay > new TimeSpan(0, 0, 0))
        {
            // If it's already Monday and the time is past 00:00, move to the next Monday
            daysUntilMonday = 7;
        }

        DateTime nextMonday = fromTime.Date.AddDays(daysUntilMonday);
        // DateTime nextMonday2Pm = nextMonday.AddHours(14); // 14:00 on that Monday

        return new DateTime(nextMonday.Year, nextMonday.Month, nextMonday.Day, 0, 0, 0, DateTimeKind.Utc);
    }
    public bool WasNextDayMidnightBetween(DateTime lastTime, DateTime now)
    {
        // Find the next day's 14:00 after lastTime
        DateTime nextDayMidnight = GetNextDayMidnight(lastTime);

        // Check if that next day's 14:00 is before now
        return nextDayMidnight <= now;
    }

    public DateTime GetNextDayMidnight(DateTime fromTime)
    {
        // Move to the next day
        DateTime nextDay = fromTime.Date.AddDays(1);
        // DateTime nextDay2Pm = nextDay.AddHours(14); // 14:00 on the next day

        return new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 0, 0, 0, DateTimeKind.Utc);
    }

    public bool WasFirstDayOfMonthBetween(System.DateTime start, System.DateTime end)
    {
        System.DateTime current = start.Date.AddDays(1);

        while (current <= end.Date)
        {
            if (current.Day == 1)
                return true;
            current = current.AddDays(1);
        }

        return false;
    }

    List<string> _sptNameList = new List<string>();
    List<Sprite> _sptList = new List<Sprite>();
    // Add at class level
    private Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();

    public Sprite GetSprite(string id)
    {
        // Return cached sprite if exists
        if (_spriteCache.ContainsKey(id))
        {
            return _spriteCache[id];
        }

        Sprite sprite = null;

        return sprite;
    }

    public Sprite GetCoinSprite()
    {
        return GetSpriteFromMultiple("Images/UI/coins", CurrentCoinEvent.ToString());
    }
    public string GetAniName(Animator animator)
    {
        AnimatorClipInfo[] array = animator.GetCurrentAnimatorClipInfo(0);
        if (array.Length == 0) return "";
        return array[0].clip.name;
    }
    public Sprite GetBackGround(int index)
    {
        string str = "Images/UI/box_item" + index;
        return Resources.Load<Sprite>(str);
    }
    public Sprite GetLongSlotBackGround(int index)
    {
        string str;
        if (index < 2) str = "Images/UI/box_item" + (index + 1);
        else if (index == 2) str = "Images/UI/box_item2ep";
        else str = "Images/UI/box_item" + index;
        return Resources.Load<Sprite>(str);
    }
    public Sprite GetRankInfoBackSprite(int index)
    {
        string str;
        if (index < 2) str = "Images/UI/textbox_rank" + (index + 1);
        else if (index == 2) str = "Images/UI/textbox_rank2ep";
        else str = "Images/UI/textbox_rank" + index;
        return Resources.Load<Sprite>(str);
    }
    public Sprite GetEquipmentSlotSprite(int index)
    {
        return GetSpriteFromMultiple("Images/UI/boxes", "boxes_" + index);
    }
    public Sprite GetDungeonKeySprite(int dungeon)
    {
        return Resources.Load<Sprite>("Images/UI/dungeonTicket" + (int)dungeon);
    }
    public DateTime GetLocalSavedDate(string key)
    {
        string strTime = PlayerPrefs.GetString(key, "");
        DateTime time;
        if (DateTime.TryParse(strTime, out time))
        {
            return time;
        }
        return new DateTime();
    }
    public void SetLocalSavedDate(string key, DateTime time)
    {
        // DateTime을 문자열로 변환해서 PlayerPrefs에 저장
        string strTime = time.ToString("o"); // ISO 8601 형식으로 변환 (권장)
        PlayerPrefs.SetString(key, strTime);
        PlayerPrefs.Save(); // 저장 내용을 즉시 저장
    }

    public Sprite GetSwordSprite(int index)
    {
        return GetSpriteFromMultiple("swords", string.Format("{0}", index + 1));
    }
    public Sprite GetSpiritSprite(int index)
    {
        return Resources.Load<Sprite>("Images/Spirit/egg" + index % 4);
    }
    public Sprite GetRuneSprite(int index)
    {
        //if (index < 0)
        //{
        //    return null;
        //    // return empty rune image
        //}
        return GetSpriteFromMultiple("images/UI/runes", string.Format("runes_{0}", index));//TheGameScript.RuneSpriteList[index];
    }
    public Sprite GetCatToySprite(int index)
    {
        if (index < 0)
        {
            return GetSpriteFromMultiple("images/UI/runes", string.Format("runes_{0}", -1));
            //return empty rune image
        }
        return null;//TheGameScript.ToySpriteList[index];
    }
    public Sprite GetShieldSprite(int index)
    {
        return GetSpriteFromMultiple("shields", string.Format("shields_{0}", index));
    }
    public Sprite GetHeroSprite(int index)
    {
        //print("get next hero set sprite " + index);
        return GetSpriteFromMultiple(string.Format("cat{0}", index), "head");//Resources.Load<Sprite>(string.Format("Heads/catHead{0}", index));
        //return GetSpriteFromMultiple("hero", string.Format("hero_{0}", index));
    }

    public Sprite GetBadgeGraySprite()
    {
        return GetSpriteFromMultiple("almost", "ui_lucky_off");
    }
    public Sprite GetBadgeGoldSprite()
    {
        return GetSpriteFromMultiple("almost", "ui_lucky");
    }
    public Sprite GetPnlGraySprite()
    {
        return GetSpriteFromMultiple("almost", "ui_pnlGrey");
    }
    public Sprite GetPnlGoldSprite()
    {
        return GetSpriteFromMultiple("almost", "ui_pnlGold");
    }
    public int GetDungeonEnemyIndex(int stage, int index)
    {
        if ((stage % 4) * 2.5f <= index)
        {
            return stage / 4;
        }
        else
        {
            return stage / 4 + 1;
        }
    }
    List<Sprite> _loadSprites = new List<Sprite>();
    public Sprite GetSpriteFromMultiple(string path, string name)
    {
        Sprite sprite = _loadSprites.Find(spt => spt.name.Equals(name));
        if (sprite) return sprite;
        // print("find sprite: " + name);
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        foreach (Sprite spt in sprites)
        {
            if (spt.name.Equals(name))
            {
                _loadSprites.Add(spt);
                return spt;
            }
        }
        return null;
    }
    public static Color HexToColor(string hex)
    {
        const System.Globalization.NumberStyles HexNumber = System.Globalization.NumberStyles.HexNumber;

        if (hex.Length < 6)
            return Color.magenta;

        hex = hex.Replace("#", "");
        byte r = byte.Parse(hex.Substring(0, 2), HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), HexNumber);
        byte a = 0xFF;
        if (hex.Length == 8)
            a = byte.Parse(hex.Substring(6, 2), HexNumber);

        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }
    private void OnApplicationPause(bool pause)
    {
        //        SaveGameData();
        //        AchievementManager.Instance.SaveAchievementData();
    }
    public void ShowEffect(string effectName, Vector2 pos)
    {
        if (TheGameScript)
        {
            TheGameScript.ShowEffect(effectName, pos);
        }
    }
    public string GetLanguageCode(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.Korean: return "ko";
            case SystemLanguage.Japanese: return "ja";
            case SystemLanguage.ChineseTraditional: return "zh-TW";
            case SystemLanguage.Vietnamese: return "vi";
            case SystemLanguage.Indonesian: return "id";
            case SystemLanguage.Thai: return "th";
            case SystemLanguage.Spanish: return "es";
            case SystemLanguage.French: return "fr";
            case SystemLanguage.Portuguese: return "pt";
            case SystemLanguage.Italian: return "it";
            case SystemLanguage.Dutch: return "nl";
            case SystemLanguage.Hindi: return "hi";
            default: return "en";
        }
    }
    public void OpenPopupAnimated(GameObject obj)
    {
        OpenPopupAnimated(obj, 1);
    }
    public void OpenPopupAnimated(GameObject obj, float scale)
    {
        OpenPopupAnimated(obj, scale, () => { });
    }
    public void OpenPopupAnimated(GameObject obj, float scale, System.Action callback)
    {
        //obj.SetActive(true);
        //var seq = LeanTween.sequence();
        //seq.append(LeanTween.scale(obj, new Vector3(scale * 0.9f, scale * 0.9f, scale * 0.9f), 0.0f));
        //seq.append(LeanTween.scale(obj, new Vector3(scale, scale, scale), 0.4f).setEaseInOutBack());
        //seq.append(callback);
    }
    public Canvas GetCanvasInCurrentScene()
    {
        Canvas canvas = null;
        foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            canvas = obj.GetComponent<Canvas>();
            if (canvas != null)
            {
                break;
            }
        }
        return canvas;
    }

    public void SetPositionY(Transform trans, float y)
    {
        Vector3 pos = trans.position;
        pos.y = y;
        trans.position = pos;
    }
    public int GetDungeonReward(int stage)
    {
        if (stage < 10)
        {
            return 1000 + (stage % 10) * 100;
        }
        else
        {
            return 2000 + stage * 10;
        }
    }
    public void SetLevelText(Text lbl, int level, int maxLevel, string name = "")
    {
        string strLevel = string.Format("Lv.{0} ", level);
        if (level >= maxLevel)
        {
            strLevel = "Lv.MAX";
        }
        lbl.text = strLevel + name;
    }
    public void SetNotification()
    {
        //TheGameScript.NotiManager.CancelAllNitofications();
        //bool isOn = PlayerPrefs.GetInt(Consts.Key_Notification, 1) == 1;
        //DateTime now = DateTime.Now;
        //if (isOn)
        //{
        //    List<int> list = new List<int>();
        //    list.Add(12);
        //    list.Add(20);
        //    list.Add(21);
        //    foreach (int hour in list)
        //    {
        //        TheGameScript.NotiManager.SendNotification(LanguageManager.GetText("monster wave"),
        //        LanguageManager.GetText("monster wave noti"),
        //        new DateTime(now.Year, now.Month, now.Day, hour, 0, 0),
        //        null,
        //        true);
        //    }
        //}
    }

    public static float GetAngle(Vector3 pos1, Vector2 pos2)
    {
        float xGap = pos2.x - pos1.x;
        float yGap = pos2.y - pos1.y;
        return Mathf.Atan2(yGap, xGap) * 180 / Mathf.PI;
    }
    public static float GetAngle(Vector2 pos1, Vector2 pos2)
    {
        float xGap = pos2.x - pos1.x;
        float yGap = pos2.y - pos1.y;
        return Mathf.Atan2(yGap, xGap) * 180 / Mathf.PI;
    }
    public static Transform GetClosest(Vector3 pos, float radius, params string[] param)
    {
        return GetClosest(pos, radius, LayerMask.GetMask(param));
    }
    public static Transform GetClosest(Vector3 pos, float radius, LayerMask mask)
    {
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        Collider[] hitColliders = Physics.OverlapSphere(pos, radius, mask);
        //Debug.Log("coll check " + hitColliders.Length);
        foreach (Collider hitCollider in hitColliders)
        {
            float distance = Vector3.Distance(pos, hitCollider.transform.position);
            if (distance < closestDistance)
            {
                closestEnemy = hitCollider.transform;
                closestDistance = distance;
            }
        }
        return closestEnemy;
    }
    public static Transform GetClosest2D(Vector3 pos, float radius, params string[] param)
    {
        int layerMask = LayerMask.GetMask(param);
        //print($"getClosest2d: {pos}/{radius}/{param.Length}/{param[0]}");
        return GetClosest2D(pos, radius, layerMask);
    }
    public static Transform GetClosest2D(Vector3 pos, float radius, LayerMask mask)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, radius, mask);
        Transform nearestUnit = null;
        float nearestDistance = float.MaxValue;
        foreach (Collider2D hit in hits)
        {
            if (!hit.gameObject.activeSelf) continue;
            //Rigidbody2D unit = hit.GetComponent<Rigidbody2D>();
            //if (unit != null)
            {
                float distance = Vector2.Distance(pos, hit.transform.position);
                if (distance < nearestDistance)
                {
                    nearestUnit = hit.transform;
                    nearestDistance = distance;
                }
            }
        }
        if (nearestUnit != null)
        {
            return nearestUnit.transform;
        }
        else return null;
    }
    public static Vector3 GetDistanceByAngleAndSpeed(double angle, float speed)
    {
        double theta = angle * Math.PI / 180;
        return new Vector3((float)Math.Cos(theta) * speed, (float)Math.Sin(theta) * speed, 0);
    }
    public float GetDistance(Transform t1, Transform t2)
    {
        return Mathf.Sqrt(Mathf.Pow(t1.position.x - t2.position.x, 2) +
                        Mathf.Pow(t1.position.y - t2.position.y, 2) +
                        Mathf.Pow(t1.position.z - t2.position.z, 2));
    }
    public Transform GetTargetInRadius(Vector3 center, float radius, LayerMask targetLayer)
    {
        Collider2D[] results = new Collider2D[10];
        int numberOfResults = Physics2D.OverlapCircleNonAlloc(center, radius, results, targetLayer);
        // if there are no targets around, we exit
        if (numberOfResults == 0)
        {
            return null;
        }

        // we go through each collider found
        float minDistance = 999999;
        Transform target = null;
        for (int i = 0; i < numberOfResults; i++)
        {
            if (results[i] == null)
            {
                continue;
            }
            float distance = (results[i].transform.position - center).sqrMagnitude;
            if (minDistance > distance)
            {
                minDistance = distance;
                target = results[i].transform;
            }
        }
        return target;
    }
    public Collider2D[] GetTargetsInRadius(Vector3 center, float radius, int targetLayer, int maxTarget = 10)
    {
        Collider2D[] results = new Collider2D[maxTarget];
        int numberOfResults = Physics2D.OverlapCircleNonAlloc(center, radius, results, targetLayer);
        if (numberOfResults == 0)
        {
            return null;
        }

        return results;
    }
    public Collider2D[] GetTargetsInRadius(Vector3 center, float radius, LayerMask targetLayer, int maxTarget = 10)
    {
        Collider2D[] results = new Collider2D[maxTarget];
        int numberOfResults = Physics2D.OverlapCircleNonAlloc(center, radius, results, targetLayer);
        // if there are no targets around, we exit
        if (numberOfResults == 0)
        {
            return null;
        }

        return results;
    }
    public List<GameObject> GetCollidings(Vector3 pos, float radius, int count, params string[] tagParam)
    {
        if (GameManager.Instance.IsSkillRangeInfinity)
        {
            radius = Mathf.Infinity;
        }

        // 미리 크기가 정해진 배열 사용
        GameObject[] selectableObjects = new GameObject[1000]; // 적절한 크기로 조정
        int totalObjects = 0;

        // FindGameObjectsWithTag 결과를 배열에 직접 복사
        foreach (var tag in tagParam)
        {
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
            System.Array.Copy(taggedObjects, 0, selectableObjects, totalObjects, taggedObjects.Length);
            totalObjects += taggedObjects.Length;
        }

        // 거리 계산과 정렬을 동시에 처리
        float[] distances = new float[totalObjects];
        int validCount = 0;

        for (int i = 0; i < totalObjects; i++)
        {
            if (selectableObjects[i] == null) continue;

            float sqrDist = (selectableObjects[i].transform.position - pos).sqrMagnitude;
            if (sqrDist > radius) continue;

            distances[validCount] = sqrDist;
            // 필요한 만큼만 정렬
            int j = validCount;
            while (j > 0 && distances[j - 1] > sqrDist)
            {
                distances[j] = distances[j - 1];
                var temp = selectableObjects[j];
                selectableObjects[j] = selectableObjects[j - 1];
                selectableObjects[j - 1] = temp;
                j--;
            }
            distances[j] = sqrDist;
            validCount++;

            if (validCount >= count) break;
        }

        // 결과 반환
        List<GameObject> result = new List<GameObject>(count);
        for (int i = 0; i < Mathf.Min(count, validCount); i++)
        {
            result.Add(selectableObjects[i]);
        }

        return result;
    }
    //public void MoveUpAndDestroy(GameObject obj, float scale, float time, float fadeOutDelay = 2)
    //{
    //    obj.transform.DOMove(new Vector3(obj.transform.position.x, obj.transform.position.y + 1, 1), time).SetEase(Ease.OutSine);
    //    ScaleAndFadeOut(obj, time, scale, time, fadeOutDelay);
    //}
    //public void FadeInAndOutUI(GameObject obj, float inTime, float outTime)
    //{
    //    var seq = LeanTween.sequence();
    //    seq.append(LeanTween.value(obj, 1, 0, 0.2f).setOnUpdate((float val) => {
    //        obj.GetComponent<MaskableGraphic>().color = new Color(0.1960784f, 0.1960784f, 0.1960784f, val);
    //    }));
    //    seq.append(0.3f);

    //    seq.append(LeanTween.value(obj, 0, 1, 0.2f).setOnUpdate((float val) => {
    //        obj.GetComponent<MaskableGraphic>().color = new Color(0.1960784f, 0.1960784f, 0.1960784f, val);
    //    }));
    //    seq.append(0.2f);
    //    seq.append(() => { FadeInAndOutUI(obj, inTime, outTime); });
    //    //Debug.Log("FadeInAndOut!");
    //}
    //public void FadeOutAndDestroy(GameObject obj, float time, float delay = 0)
    //{
    //    FadeOutAndDestroy fadeOut = obj.AddComponent<FadeOutAndDestroy>();
    //    fadeOut.StartFadeOut(time, delay);
    //}
    //public void ScaleAndFadeOut(GameObject obj, float scaleTime, float scale, float fadeTime, float fadeOutDelay = 1)
    //{
    //    FadeOutAndDestroy(obj, fadeTime, fadeOutDelay);
    //    obj.transform.DOScale(scale, scaleTime).SetEase(Ease.OutElastic);
    //}
    public string GetTimeLeftStringColon(double seconds)
    {
        seconds = Math.Max(seconds, 0);
        int sec = (int)seconds;
        string result = (sec % 60).ToString("00");
        int min = sec / 60;
        int hour = min / 60;
        min = min % 60;
        result = min.ToString("00") + ":" + result;
        if (hour > 0)
        {
            result = hour.ToString("00") + ":" + result;
        }
        return result;
    }
    public string GetTimeLeftString(long seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        //if (seconds >= 172800)
        if (seconds >= 86400)
        {
            //Debug.Log(string.Format("{0}/{1}", seconds, time.Seconds));
            if (LanguageManager.Instance.GetCurrentLanguage() == SystemLanguage.Korean)
            {
                //return string.Format("{0}일 {1}시간 {2}분", time.Days, time.Hours, time.Minutes);
                return string.Format("{0}일 {1}시간", time.Days, time.Hours);
            }
            else
            {
                //return string.Format("{0}D:{1}h:{2}m", time.Days, time.Hours, time.Minutes);
                return string.Format("{0}D:{1}h", time.Days, time.Hours);
            }
        }
        else if (seconds >= 3600)
        {
            if (LanguageManager.Instance.GetCurrentLanguage() == SystemLanguage.Korean)
            {
                //return string.Format("{0}시간 {1}분 {2}초", time.Hours, time.Minutes, time.Seconds);
                return string.Format("{0}시간 {1}분", time.Hours, time.Minutes);
            }
            else
            {
                //return string.Format("{0}h:{1}m:{2}s", time.Hours, time.Minutes, time.Seconds);
                return string.Format("{0}h:{1}m", time.Hours, time.Minutes);
            }
        }
        else
        {
            if (LanguageManager.Instance.GetCurrentLanguage() == SystemLanguage.Korean)
            {
                string strResult = string.Format("{0}분", seconds / 60);

                if (seconds % 60 > 0)
                {
                    string strSec = string.Format(" {0}초", seconds % 60);
                    strResult += strSec;
                }
                return strResult;
            }
            else
            {
                return string.Format("{0:00}:{1:00}", seconds / 60, seconds % 60);
            }

        }
    }
    public string GetTimeLeftStringOnlyMin(long seconds)
    {
        if (LanguageManager.Instance.GetCurrentLanguage() == SystemLanguage.Korean)
        {
            string strResult = string.Format("{0}분", seconds / 60);

            return strResult;
        }
        else
        {
            return string.Format("{0}min", seconds / 60);
        }
    }
    public string GetNumberText(double number)
    {
        number = System.Math.Floor(number);
        if (number == 0)
        {
            return "0";
        }
        string str = string.Empty;
        int counter = 0;
        if (LanguageManager.Instance.GetCurrentLanguage() == SystemLanguage.Korean) // korean
        {
            if (number >= 10000000000000000)
            {
                str += string.Format("{0}경", (int)(number / 10000000000000000));
                counter++;
            }
            number = number % 10000000000000000;
            //str += number / 1000000000000;
            if (number >= 1000000000000)
            {
                str += string.Format("{0}조", (int)(number / 1000000000000));
                counter++;
            }
            if (counter > 1) return str;
            number = number % 1000000000000;
            //str += number / 100000000;
            if (number >= 100000000)
            {
                str += string.Format("{0}억", (int)(number / 100000000));
                counter++;
            }
            if (counter > 1) return str;
            number = number % 100000000;
            //str += number / 10000;
            if (number >= 10000)
            {
                str += string.Format("{0}만", (int)(number / 10000));
                counter++;
            }
            if (counter > 1) return str;
            number = number % 10000;
            if (number > 0)
            {
                str += number.ToString();
            }
        }
        else // english
        {
            if (number >= 1000000000000000)
            {
                str += string.Format("{0}Q ", (int)(number / 1000000000000000));
                counter++;
            }
            number = number % 1000000000000000;
            //str += number / 1000000000000;
            if (number >= 1000000000000)
            {
                str += string.Format("{0}T ", (int)(number / 1000000000000));
                counter++;
            }
            if (counter > 1) return str;
            number = number % 1000000000000;
            //str += number / 100000000;
            if (number >= 1000000000)
            {
                str += string.Format("{0}B ", (int)(number / 1000000000));
                counter++;
            }
            if (counter > 1) return str;
            number = number % 1000000000;
            //str += number / 10000;
            if (number >= 1000000)
            {
                str += string.Format("{0}M ", (int)(number / 1000000));
                counter++;
            }
            if (counter > 1) return str;
            number = number % 1000000;
            //str += number / 10000;
            if (counter > 0)
            {
                if (number >= 1000)
                {
                    str += string.Format("{0}K", (int)(number / 1000));
                    counter++;
                }
                if (counter > 1) return str;
                number = number % 1000;
            }
            str += number.ToString();
        }
        return str;
    }

    public float GetDegreeAngle(Vector3 from, Vector3 to)
    {
        float xGap = to.x - from.x;
        float yGap = to.y - from.y;
        float angle = Mathf.Atan2(yGap, xGap);

        return angle * 180 / 3.14f;
    }

    public Vector3 GetAngle(Vector3 from, Vector3 to)
    {
        //float rawAngle = 90;
        //float angle = -rawAngle;

        //float rad = angle * 3.14f / 180;
        //Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
        //return dir;

        //return Vector3.Angle(targetDir, transform.forward);
        return to - from;
    }


    public UIScript GetUIScript()
    {
        return TheGameScript.GetComponent<UIScript>();
    }

    //public int GetMasterStarCount(UnitInfo info)
    //{
    //    int count = info.Star[0];
    //    count += info.Star[1] * 2;
    //    count += info.Star[2] * 4;
    //    count += info.Star[3] * 8;
    //    count += info.Star[4] * 16;
    //    return count;
    //}

    public int GetGuildUpgradeMaxExp(int level)
    {
        int maxExp = 100;
        for (int i = 0; i < level; i++)
        {
            maxExp += 100;
        }
        return maxExp;
    }

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z = 0)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }
    public static void ShowGlow(Transform parent, Vector3 pos = default)
    {
        GameObject glowEffect = Instantiate(Resources.Load<GameObject>("effect/btnLight_UI/btnLight_Effect"), parent);
        glowEffect.AddComponent<DestroyAfterSec>().DestroyAfter(1.5f);
        glowEffect.transform.localPosition = pos;
    }
    public static void ShowInstanceMessage(InstanceMessages notEnought)
    {
        string key = "not enough currency";
        if (notEnought == InstanceMessages.NotEnoughTicket) key = "not enough ticket";
        else if (notEnought == InstanceMessages.NotEnoughMaterial) key = "not enough material";
        else if (notEnought == InstanceMessages.Purchased) key = "just purchase success";
        else if (notEnought == InstanceMessages.PurchaseFailed) key = "purchase failed";
        else if (notEnought == InstanceMessages.PurchaseSuccess) key = "purchase success";
        else if (notEnought == InstanceMessages.BuyLimit) key = "but limit reach";
        else if (notEnought == InstanceMessages.FailToUse) key = "fail to use";
        else if (notEnought == InstanceMessages.AdsNotReady) key = "ads not ready";
        else if (notEnought == InstanceMessages.ServerFailed) key = "server failed";
        else if (notEnought == InstanceMessages.AdsLimitArrived) key = "ads limit arrived";
        else if (notEnought == InstanceMessages.ServerAdjusting) key = "in server adjustment";
        else if (notEnought == InstanceMessages.NoInternet) key = "no internet";
        _instance.TheGameScript.TheUIScript.ShowInstantMessage(key);
    }
    public static void ShowInstanceMessage(string msg)
    {
        //print("show instance msg: " + msg);
        if (_instance.TheGameScript) _instance.TheGameScript.TheUIScript.ShowInstantMessage(msg);
        else if (_instance.TheTitle) _instance.TheTitle.ShowInstantMessage(msg);
    }
    public static void ShowShinyImage(Transform parent)
    {
        Transform shiny = parent.Find("ShinyImage");
        shiny.gameObject.SetActive(true);
        shiny.GetComponent<Animator>().Play(0, -1, 0);
    }
    public static void HideShinyImage(Transform parent)
    {
        Transform shiny = parent.Find("ShinyImage");
        shiny.gameObject.SetActive(false);
    }
    public static void ShowOkPopup(string descKey, UnityAction callback)
    {
        if (_instance.TheTitle) _instance.TheTitle.ShowOkDialog(descKey, callback);
        else _instance.TheGameScript.TheUIScript.ShowOkDialog(descKey, callback);
    }

    /// <summary>
    /// args: item,count,item,count,....
    /// </summary>
    /// <param name="args"></param>
    public static void ShowReward(params string[] args)
    {
        _instance.TheGameScript.TheUIScript.ShowRewardReceived(args);
    }
    public static void ShowDialog(string descKey, UnityAction callback)
    {
        _instance.TheGameScript.TheUIScript.ShowDialog(descKey, callback);
    }
    public static void ShowCancelDialog(string descKey, UnityAction callback)
    {
        _instance.TheGameScript.TheUIScript.ShowCancelDialog(descKey, callback);
    }
    public static void ShowItemDetail(string itemID, string count = "1")
    {
        _instance.TheGameScript.TheUIScript.ShowItemDetail(itemID, count);
    }
    public static void ShowDialog(string titleKey, string descKey, UnityAction callback)
    {
        _instance.TheGameScript.TheUIScript.ShowDialog(titleKey, descKey, callback);
    }

    public static void ShowIndicator()
    {
        if (_instance != null)
        {
            _instance.TheGameScript.TheUIScript.ShowIndicator();
        }
    }

    public static void ShowIndicator(float closeLater)
    {
        if (_instance != null)
        {
            _instance.TheGameScript.TheUIScript.ShowIndicator(closeLater);
        }
    }
    public static void HideIndicator()
    {
        if (_instance != null)
        {
            _instance.TheGameScript.TheUIScript.HideIndicator();
        }
    }

    public static DateTime GetTimeFromStamp(long timestamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
    }
    public static long GetTimeStampFromDateTime(DateTime time)
    {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        TimeSpan timeSinceEpoch = time.ToUniversalTime() - epoch;
        return (long)timeSinceEpoch.TotalSeconds;
    }
    public static bool LayerContains(LayerMask layerMask, int layer)
    {
        return (layerMask.value & (1 << layer)) > 0;
    }
    public List<int> FillList(List<int> list, int index, int defaultValue = 0)
    {
        //print("guard");
        if (list == null)
        {
            //print("guard1");
            list = new List<int>();
        }
        for (int i = 0; i <= index; i++)
        {
            //print("guard index: " + i);
            //print("guard count: " + list.Count);
            if (list.Count <= index)
            {
                //print("guard3");
                list.Add(defaultValue);
            }
        }
        //print("guard4");
        return list;
    }
    //public List<int> FillList(List<int> list, int index, int defaultValue = 0)
    //{
    //    if (list == null)
    //    {
    //        list = new List<int>();
    //    }
    //    for(int i = 0; i <= index; i++)
    //    {
    //        if (list.Count <= index)
    //        {
    //            list.Add(defaultValue);
    //        }
    //    }
    //    return list;
    //}
    public List<string> FillList(List<string> list, int index, string defaultValue = "")
    {
        if (list == null)
        {
            list = new List<string>();
        }
        for (int i = 0; i <= index; i++)
        {
            if (list.Count <= index)
            {
                list.Add(defaultValue);
            }
        }
        return list;
    }

    public static void SetAudioMixerVolume(AudioMixer mixer, string value, float volume)
    {
        if (mixer == null)
            return;

        mixer.SetFloat("MasterVolume", Mathf.Lerp(-80, 0, volume));
    }
    public static string RemoveStringOnce(string strFull, string strToRemove)
    {
        int index = strFull.IndexOf(strToRemove);
        string result = strFull;
        if (index >= 0)
        {
            result = strFull.Remove(index, strToRemove.Length);
        }
        return result;
    }
    public static Sprite GetDungeonTicketSprite(int index)
    {
        return Instance.GetSpriteFromMultiple("Images/UI/", string.Format("dungeonTicket{0}", index));
    }
    public static void AddQuest(string questID, int count = 1)
    {

    }


    public DateTime GetCostumeEventEndDate(int index)
    {
        if (index == 0) return new DateTime(2025, 1, 15, 23, 59, 59);
        return new DateTime();
    }
    static DateTime _lastTime;
    public bool IsPurchased(string id)
    {
        //string iapList = SaveData.Instance.Data.iap_list.ToString();
        //print("iapList: " + iapList);
        //string key = "+" + Consts.GetIAPIndex(id) + "+";
        //print("key: " + key);
        //return iapList.Contains(key);
        // return SaveData.Instance.Data.GetIAPCount(id) > 0;
        return false;
    }
    public string GetCostumeCategory(string id)
    {
        if (id.StartsWith("bb_") || id.StartsWith("wb_") || id.StartsWith("mb_")) return "bd";
        else if (id.StartsWith("bh_") || id.StartsWith("wh_") || id.StartsWith("mh_")) return "ha";
        else return id.Substring(0, 2);
    }
    //public void SetCostume(Transform parent, int body, int hair, int weapon, int effect)
    //{
    //    //if (string.IsNullOrEmpty(costumeID) || costumeID.Length < 2) return;

    //    //CostumeData cData = DataManager.Instance.GetCostumeData(costumeID);
    //    //print($"cData:{cData}/id:{costumeID}");
    //    //GameObject obj = Instantiate(Resources.Load<GameObject>(cData.resource), parent);

    //    parent.GetComponent<ParentModel>().SetCostume(body, hair, weapon, effect);
    //    //if (cData.cos_group == (int)CostumeTypes.Effect)
    //    //{
    //    //    Transform previousEffect = parent.Find("effect");
    //    //    if (previousEffect) Destroy(previousEffect.gameObject);

    //    //    obj.name = "effect";
    //    //}

    //}

    public Sprite GetShopItemSprite(string str)
    {
        if (str.Contains("__"))
        {
            //print("str: " + str);
            //print("str index: " + str.IndexOf("__"));
            string path = str.Substring(0, str.IndexOf("__"));
            string item = str.Substring(str.IndexOf("__") + 2);
            //print("sprite path: " + path + "/" + item);
            return GetSpriteFromMultiple(path, item);
        }
        else
        {
            return Resources.Load<Sprite>(str);
        }
    }

    public string GetDataForRank()
    {
        return string.Empty;
    }

    public string GetRomeNumber(int num)
    {
        string result = GetRomeNumberUnder10(num);


        if (num / 10 > 0)
        {
            result = "X" + result;
        }
        result = GetRomeNumberUnder10(num / 10) + result;
        return result;
    }

    public string GetRomeNumberUnder10(int num)
    {
        string result = "";
        if (num % 5 > 0)
        {
            for (int i = 0; i < num; i++)
            {
                result += "I";
            }
        }

        if (num % 10 > 5)
        {
            result = "V" + result;
        }
        return result;
    }

    public void ExecuteCallback(float delay, UnityAction callback)
    {
        StartCoroutine(ExecuteSchedule(delay, callback));
    }
    IEnumerator ExecuteSchedule(float delay, UnityAction callback)
    {
        yield return new WaitForSeconds(delay);
        try
        {
            callback?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    public void ExecuteCallback(float delay, GameObject obj, UnityAction<GameObject> execute, UnityAction<GameObject> callback)
    {
        execute?.Invoke(obj);
        StartCoroutine(ExecuteScheduleObj(delay, obj, callback));
    }
    IEnumerator ExecuteScheduleObj(float delay, GameObject obj, UnityAction<GameObject> callback)
    {
        yield return new WaitForSeconds(delay);
        try
        {
            callback?.Invoke(obj);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    public static void ShakeCamera()
    {
        if (_instance.TheGameScript)
        {
            CameraShake shake = _instance.TheGameScript.TheCamera.GetComponent<CameraShake>();
            if (shake) shake.Shake(0.15f);
        }
    }
    public void StartEnemyKill()
    {
        _huntStartCount = DateTime.Now;
        _enemyDeadCount = 0;
    }
    public void AddEnemyKill()
    {
        _enemyDeadCount++;
        //TimeSpan time = DateTime.Now - _huntStartCount;
        //print($"time: {time.TotalMinutes}/ kill: {_enemyDeadCount}/EnemyPerMin: {_enemyDeadCount*1f/time.TotalMinutes}");
    }
    public Transform FindChild(Transform parent, string str)
    {
        Transform target;
        target = parent.Find(str);
        //print($"find {parent.name}/{str}");
        if (target) return target;
        foreach (Transform child in parent)
        {
            Transform result = FindChild(child, str);
            if (result) return result;
        }
        return target;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dmgPool"></param>
    /// <param name="str"></param>
    /// <param name="pos"></param>
    /// <param name="cri">-3: continuousHit, -2: cutHit</param>
    /// <param name="option"></param>
    /// <returns></returns>
    public GameObject ShowDamageFont(ObjectPool dmgPool, string str, Vector3 pos, int cri = 0, int option = 0)
    {
        // print("show dmg pos: " + pos.x + "/" + pos.y + "/" + pos.z);
        // print("show damage font: " + str + "cri: " + cri + "option: " + option);
        GameObject obj = dmgPool.GetObject();
        TextMeshPro tmp;
        // tmp.color = color;
        // tmp.fontSize = cri > 0 ? 4.5f : 3.5f;
        // tmp.enableVertexGradient = cri > 0;
        float range = 2;
        obj.transform.position = pos + new Vector3(UnityEngine.Random.Range(-range / 2, range / 2), UnityEngine.Random.Range(-range / 4, range / 4), 0);
        Animator ani = obj.GetComponent<Animator>();
        Transform tfText = obj.transform.Find("Text");
        bool isCutHit = cri == -2 || option == 1;
        bool isContinuousHit = cri == -3;
        tfText.transform.Find("SubTex").Find("Slash").gameObject.SetActive(isCutHit);
        tfText.transform.Find("SubTex").Find("Combo").gameObject.SetActive(isContinuousHit);
        if (isCutHit)
        {
            tmp = tfText.Find("Damage").GetComponent<TextMeshPro>();
            ani.Play("Damage", -1, 0);
        }
        else if (cri == 0 || isCutHit || isContinuousHit)
        {
            tmp = tfText.Find("Damage").GetComponent<TextMeshPro>();
            ani.Play("Damage", -1, 0);
        }
        else if (cri == -1)
        {
            tmp = tfText.Find("Hit").GetComponent<TextMeshPro>();
            ani.Play("Hit", -1, 0);
        }
        else
        {
            tmp = tfText.Find("Crii").GetComponent<TextMeshPro>();
            ani.Play("Damage_cri", -1, 0);
        }
        // ExecuteCallback(0.1f, null, () =>
        // {
        tfText.Find("Damage").gameObject.SetActive((option == 0 && cri == 0) || isCutHit || isContinuousHit);
        tfText.Find("Hit").gameObject.SetActive(cri == -1);
        tfText.Find("Crii").gameObject.SetActive(cri > 0);
        tfText.Find("backi").gameObject.SetActive(cri > 0);
        // tfText.Find("Puncturei").gameObject.SetActive(option == 1 || cri == -2);
        // });
        // BigNum num = new BigNum(str);
        // if (num >= 0) tmp.text = num;
        tmp.text = str;
        obj.GetComponent<ObjectPoolItem>().StartTimer(2);
        // obj.transform.Find("Font").Find("Text").Find("back").gameObject.SetActive(cri > 0);
        return obj;
    }
    public DateTime ConvertTimeToLocal(DateTime time)
    {
        long longTime = GetTimeStampFromDateTime(time);
        return DateTimeOffset.FromUnixTimeSeconds(longTime).ToLocalTime().DateTime;
    }

    public IEnumerator MoveCamera(Camera cam, Vector3 startPos, Vector3 targetPos, float duration)
    {
        if (cam == null)
        {
            Debug.LogError("Main Camera not found!");
            yield break;
        }

        cam.transform.position = startPos;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            cam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        cam.transform.position = targetPos; // 정확한 위치 보정
    }

    public UIAlphaElements CollectAlphaElements(Transform parent)
    {
        return new UIAlphaElements
        {
            images = new List<Image>(parent.GetComponentsInChildren<Image>(true)),
            texts = new List<Text>(parent.GetComponentsInChildren<Text>(true)),
            tmpros = new List<TextMeshProUGUI>(parent.GetComponentsInChildren<TextMeshProUGUI>(true)),
            renderers = new List<SpriteRenderer>(parent.GetComponentsInChildren<SpriteRenderer>(true))
        };
    }

    public void SetAlpha(UIAlphaElements elements, float alpha)
    {
        foreach (var img in elements.images)
        {
            if (img)
            {
                Color c = img.color;
                img.color = new Color(c.r, c.g, c.b, alpha);
            }
        }

        foreach (var txt in elements.texts)
        {
            if (txt)
            {
                Color c = txt.color;
                txt.color = new Color(c.r, c.g, c.b, alpha);
            }
        }

        foreach (var tmp in elements.tmpros)
        {
            if (tmp)
            {
                Color c = tmp.color;
                tmp.color = new Color(c.r, c.g, c.b, alpha);
            }
        }

        foreach (var sr in elements.renderers)
        {
            if (sr)
            {
                Color c = sr.color;
                sr.color = new Color(c.r, c.g, c.b, alpha);
            }
        }
    }



    public void SetAlphaFadeOut(Transform parent, float alpha)
    {
        // print("set alpha fade out: " + parent.name + " alpha: " + alpha);
        Image[] imgs = parent.GetComponentsInChildren<Image>(true);
        foreach (Image img in imgs)
        {
            if (img && img.color.a > alpha)
            {
                Color color = img.color;
                img.color = new Color(color.r, color.g, color.b, alpha);
            }
        }
        Text[] lbls = parent.GetComponentsInChildren<Text>(true);
        foreach (Text lbl in lbls)
        {
            if (lbl && lbl.color.a > alpha)
            {
                Color color = lbl.color;
                lbl.color = new Color(color.r, color.g, color.b, alpha);
            }
        }
        TextMeshProUGUI[] textmesh = parent.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI tf in textmesh)
        {
            if (tf && tf.color.a > alpha)
            {
                Color color = tf.color;
                tf.color = new Color(color.r, color.g, color.b, alpha);
            }
        }
        SpriteRenderer[] renderers = parent.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer && renderer.color.a > alpha)
            {
                Color color = renderer.color;
                //print($"set alpha1: {renderer.name}/{renderer.color.a}/{alpha}");
                renderer.color = new Color(color.r, color.g, color.b, alpha);
                //print($"set alpha2: {renderer.name}/{renderer.color.a}/{alpha}");
            }
        }
    }


    public void SetAlphaFadeIn(Transform parent, float alpha)
    {
        Image[] imgs = parent.GetComponentsInChildren<Image>(true);
        foreach (Image img in imgs)
        {
            if (img && img.color.a < alpha)
            {
                Color color = img.color;
                img.color = new Color(color.r, color.g, color.b, alpha);
            }
        }
        Text[] lbls = parent.GetComponentsInChildren<Text>(true);
        foreach (Text lbl in lbls)
        {
            if (lbl && lbl.color.a < alpha)
            {
                Color color = lbl.color;
                lbl.color = new Color(color.r, color.g, color.b, alpha);
            }
        }
        TextMeshProUGUI[] textmesh = parent.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI tf in textmesh)
        {
            if (tf && tf.color.a < alpha)
            {
                Color color = tf.color;
                tf.color = new Color(color.r, color.g, color.b, alpha);
            }
        }
        SpriteRenderer[] renderers = parent.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer && renderer.color.a < alpha)
            {
                Color color = renderer.color;
                renderer.color = new Color(color.r, color.g, color.b, alpha);
            }
        }
    }
    public string FilterStatName(string stat)
    {
        //print("key: " + key);
        if (stat.Equals("hp")) return "s_hp";
        else if (stat.Equals("hp recover")) return "s_hpGen";
        else if (stat.Equals("s_cutHitDam") || stat.Equals("s_talismanDam")) return "cut hit dmg";
        else if (stat.Equals("s_continuousHit")) return "continuous hit";
        else if (stat.Equals("s_continuousHitDam")) return "continuous hit dmg";
        else if (stat.Equals("s_cutHit")) return "cut hit";
        else if (stat.Equals("s_mask") || stat.Equals("s_maskSkill")) return "mask skill dmg";
        else if (string.IsNullOrEmpty(stat)) return "-";
        return stat;
    }
    public Vector2 GetRandomPos()
    {
        float totalWidth = MapRightBottom.x - MapLeftTop.x;
        float totalHeight = MapLeftTop.y - MapRightBottom.y;
        float halfHeight = MapRightBottom.y + totalHeight / 2f;

        // 상단과 하단의 총 확률 계산
        int totalUpProb = Spawn_Up_1 + Spawn_Up_2 + Spawn_Up_3 + Spawn_Up_4;
        int totalDownProb = Spawn_Down_1 + Spawn_Down_2 + Spawn_Down_3 + Spawn_Down_4;
        int totalProb = totalUpProb + totalDownProb;

        // 상단/하단 선택
        bool isUpperHalf = UnityEngine.Random.Range(0, totalProb) < totalUpProb;

        if (isUpperHalf)
        {
            // 상단 영역 4구역 중 하나 선택
            int upRand = UnityEngine.Random.Range(0, totalUpProb);
            int section;

            if (upRand < Spawn_Up_1) section = 0;
            else if (upRand < Spawn_Up_1 + Spawn_Up_2) section = 1;
            else if (upRand < Spawn_Up_1 + Spawn_Up_2 + Spawn_Up_3) section = 2;
            else section = 3;

            float quarterWidth = totalWidth / 4f;
            float x = UnityEngine.Random.Range(
                MapLeftTop.x + quarterWidth * section,
                MapLeftTop.x + quarterWidth * (section + 1)
            );

            return new Vector2(x, UnityEngine.Random.Range(halfHeight, MapLeftTop.y));
        }
        else
        {
            // 하단 영역 4구역 중 하나 선택
            int downRand = UnityEngine.Random.Range(0, totalDownProb);
            int section;

            if (downRand < Spawn_Down_1) section = 0;
            else if (downRand < Spawn_Down_1 + Spawn_Down_2) section = 1;
            else if (downRand < Spawn_Down_1 + Spawn_Down_2 + Spawn_Down_3) section = 2;
            else section = 3;

            float quarterWidth = totalWidth / 4f;
            float x = UnityEngine.Random.Range(
                MapLeftTop.x + quarterWidth * section,
                MapLeftTop.x + quarterWidth * (section + 1)
            );

            return new Vector2(x, UnityEngine.Random.Range(MapRightBottom.y, halfHeight));
        }
    }
    public void PlaySound(string path)
    {
        // Resources 경로에서 오디오 클립 로드
        AudioClip clip = Resources.Load<AudioClip>(path);

        if (clip == null)
        {
            Debug.LogWarning($"[PlaySound] 사운드 클립을 찾을 수 없습니다: {path}");
            return;
        }
        // 사운드 풀에서 재생
        SoundPool pool = SoundPool.Instance;
        if (pool != null)
        {
            pool.PlaySound(clip, defaultSFXMixer);
            // print("play sound from pool: " + clip.name);
        }
        else
        {
            // 풀 없으면 임시 객체로 재생
            GameObject soundObj = new GameObject("SoundPlayer_" + clip.name);
            AudioSource audioSource = soundObj.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = defaultSFXMixer;
            audioSource.clip = clip;
            audioSource.Play();
            print("play sound: " + clip.name);

            UnityEngine.Object.Destroy(soundObj, clip.length);
        }

    }
    public void PlaySound(AudioClip clip)
    {
        PlaySound(clip, Resources.Load<AudioMixerGroup>("Settings/SFX"));
    }
    public void PlaySound(AudioClip clip, AudioMixerGroup mixer)
    {
        // print("PlaySound: " + clip.name);
        if (!clip) return;
        GameObject soundObj = new GameObject("SoundPlayer");
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixer; // SFX로 이름 지은 믹서 설정
        audioSource.clip = clip;
        audioSource.Play();
        Destroy(soundObj, clip.length); // 소리 재생이 끝난 뒤에 임시 오브젝트 파괴
    }

    // Add cleanup method to clear cache if needed
    public void ClearSpriteCache()
    {
        _spriteCache.Clear();
    }

    // Optional: Add method to preload commonly used sprites
    public void PreloadCommonSprites()
    {
        string[] commonSprites = new string[]
        {
            "Images/UI/exp",
            // Add other commonly used sprite paths
        };

        foreach (string path in commonSprites)
        {
            if (!_spriteCache.ContainsKey(path))
            {
                Sprite sprite = Resources.Load<Sprite>(path);
                if (sprite != null)
                {
                    _spriteCache[path] = sprite;
                }
            }
        }
    }

    public int ExtractDigits(string input)
    {
        string digitsOnly = new string(input.Where(char.IsDigit).ToArray());

        if (int.TryParse(digitsOnly, out int number))
        {
        }
        else
        {
            Debug.LogWarning("숫자 변환 실패");
        }

        return number;
    }

}

public class SkillInfo
{
    public int Index;
    public int Level;
    public SkillInfo(string str)
    {
        // Debug.Log("skillInfo: " + str);

        string[] strs = str.Split('_');
        Index = int.Parse(strs[0]);
        if (strs.Length > 0) Level = int.Parse(strs[1]);
    }
}