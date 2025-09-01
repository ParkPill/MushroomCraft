using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class SNMenus
{
    [MenuItem("GameObject/StudioNAP/Joystick")]
    public static void MakeJoystick()
    {
        // 캔버스가 있는지 확인하고 없으면 생성
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            // EventSystem이 있는지 확인하고 없으면 생성
            if (Object.FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
            }
        }

        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/Joystick")) as GameObject;
        CreateObject(obj);
    }
    [MenuItem("GameObject/StudioNAP/Text")]
    public static void MakeText()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/Text")) as GameObject;
        obj.name = "Text";
        CreateObject(obj);
    }

    [MenuItem("GameObject/StudioNAP/ExpandableText")]
    public static void MakeExpandableText()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/ExpandableTextNew")) as GameObject;
        obj.name = "ExpandableText";
        CreateObject(obj);
    }
    [MenuItem("GameObject/StudioNAP/TextMeshPro")]
    public static void MakeTextMeshPro()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/TextMeshPro")) as GameObject;
        CreateObject(obj);
    }
    [MenuItem("GameObject/StudioNAP/TextWithIconButton")]
    public static void MakeButton()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/TextWithIconButton")) as GameObject;
        CreateObject(obj);
    }
    [MenuItem("GameObject/StudioNAP/NoHandleSlider")]
    public static void MakeNoHandleSlider()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/NoHandleSlider")) as GameObject;
        CreateObject(obj);
    }
    [MenuItem("GameObject/StudioNAP/HoldButton")]
    public static void MakeHoldButton()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/HoldButton")) as GameObject;
        CreateObject(obj);
    }

    [MenuItem("GameObject/StudioNAP/TextWithBack")]
    public static void MakeTextWithBack()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/TextWithBack")) as GameObject;
        CreateObject(obj);
    }

    [MenuItem("GameObject/StudioNAP/ImageButton")]
    public static void MakeImageButton()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/ImageButton")) as GameObject;
        CreateObject(obj);
    }
    [MenuItem("GameObject/StudioNAP/TextButton")]
    public static void MakeTextButton()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/TextButton")) as GameObject;
        CreateObject(obj);
    }
    [MenuItem("GameObject/StudioNAP/JustButton")]
    public static void MakeJustButton()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/JustButton")) as GameObject;
        CreateObject(obj);
    }
    [MenuItem("GameObject/StudioNAP/RewardIcon")]
    public static void MakeRewardIcon()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/RewardIcon2")) as GameObject;
        CreateObject(obj);
    }
    [MenuItem("GameObject/StudioNAP/RewardIconWithNumber")]
    public static void MakeRewardIconWithNumber()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/RewardIconLong")) as GameObject;
        CreateObject(obj);
    }
    [MenuItem("GameObject/StudioNAP/QuestLock")]
    public static void MakeQuestLock()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/QuestLock")) as GameObject;
        CreateObject(obj);
        obj.transform.localPosition = Vector3.zero;
    }
    [MenuItem("GameObject/StudioNAP/RewardPackButton")]
    public static void MakeRewardPackButton()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/RewardPackButton")) as GameObject;
        CreateObject(obj);
        obj.transform.localPosition = Vector3.zero;
    }
    [MenuItem("GameObject/StudioNAP/EnableButton")]
    public static void MakeEnableButton()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/EnableButton")) as GameObject;
        CreateObject(obj);
        obj.transform.localPosition = Vector3.zero;
    }
    [MenuItem("GameObject/StudioNAP/TabButton")]
    public static void MakeTabButton()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/TabButton")) as GameObject;
        CreateObject(obj);
        obj.transform.localPosition = Vector3.zero;
    }
    [MenuItem("GameObject/StudioNAP/EnableImage")]
    public static void MakeEnableSprite()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/EnableImage")) as GameObject;
        CreateObject(obj);
        obj.transform.localPosition = Vector3.zero;
    }
    [MenuItem("GameObject/StudioNAP/HScrollView")]
    public static void MakeHScrollView()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/HScrollView")) as GameObject;
        obj.name = "sv";
        CreateObject(obj);
    }
    [MenuItem("GameObject/StudioNAP/VScrollView")]
    public static void MakeVScrollView()
    {
        GameObject obj = PrefabUtility.InstantiatePrefab(Resources.Load("Prefab/UI/VScrollView")) as GameObject;
        obj.name = "sv";
        CreateObject(obj);
    }
    public static void CreateObject(GameObject obj)
    {
        obj.transform.parent = Selection.activeTransform;
        obj.transform.localScale = Vector3.one;

        SceneView lastView = SceneView.lastActiveSceneView;
        obj.transform.position = lastView ? lastView.pivot : Vector3.zero;
        if (obj.transform.localPosition.z != 0) obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0);

        StageUtility.PlaceGameObjectInCurrentStage(obj);
        GameObjectUtility.EnsureUniqueNameForSibling(obj);

        Undo.RegisterCreatedObjectUndo(obj, $"Create Object: {obj.name}");
        Selection.activeGameObject = obj;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}