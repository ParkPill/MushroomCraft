using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace StudioNAP
{
    public class PopupManager : MonoBehaviour
    {
        private static PopupManager _instance;
        public static PopupManager Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = new GameObject("PopupManager").AddComponent<PopupManager>();
                    // DontDestroyOnLoad(_instance);
                }
                return _instance;
            }
        }
        public bool IsHold = false;
        public List<GameObject> PopupList = new List<GameObject>();
        public Canvas MainCanvas;
        public Camera MainCam;
        public Camera UICam;
        //public Camera SecondCam;
        //public RenderTexture MainViewTexture;
        public Transform PopupCanvas;
        public List<Transform> CanvasList = new List<Transform>();
        bool _isPreloaded = false;
        public bool BlockClose = false;
        public string ExitMessage = "Do you want to EXIT?";
        void Start()
        {
            _instance = this;

        }
        public GameObject OpenPopupAndGet(string popup)
        {
            if (IsHold) return null;
            GameObject obj = InstantiatePopup(popup);
            UpdateIt();
            return obj;
        }

        public T Open<T>(string popup) where T : Component
        {
            GameObject obj = OpenPopupAndGet(popup);
            return obj.GetComponent<T>();
        }
        public Transform GetCurrentParent()
        {
            return CanvasList[CanvasList.Count - 1];
        }
        public Transform GetNextParent()
        {
            CheckCam();
            // if (!PopupCanvas) return GameObject.Find("Canvas").transform;
            if (PopupList.Count == 0 && CanvasList.Count == 0)
            {
                PopupCanvas = Instantiate(Resources.Load<GameObject>("Prefab/UI/PopupCanvas")).transform;
                Canvas canvas = PopupCanvas.GetComponent<Canvas>();
                canvas.renderMode = PopupList.Count > 0 ? RenderMode.ScreenSpaceCamera : RenderMode.ScreenSpaceOverlay;
                canvas.worldCamera = UICam;
                canvas.planeDistance = 1;
                CanvasList.Add(PopupCanvas);
                return PopupCanvas;
            }
            else
            {
                if (CanvasList.Count < PopupList.Count + 1)
                {
                    GameObject obj = Instantiate(Resources.Load<GameObject>("Prefab/UI/PopupCanvas"));
                    MainCanvas.renderMode = PopupList.Count > 0 ? RenderMode.ScreenSpaceCamera : RenderMode.ScreenSpaceOverlay;
                    MainCanvas.worldCamera = UICam;
                    MainCanvas.planeDistance = 1;
                    CanvasList.Add(obj.transform);
                    return obj.transform;
                }
                else
                {
                    return CanvasList[PopupList.Count];
                }
            }
        }
        public Transform Find(string popup)
        {
            foreach (var item in PopupList)
            {
                if (item.name.Equals(popup)) return item.transform;
            }
            return null;
        }
        GameObject InstantiatePopup(string popup)
        {

            GameObject obj = Instantiate(Resources.Load<GameObject>("Prefab/UI/Popup/" + popup), GetNextParent());
            obj.name = popup;
            PopupBase pb = obj.GetComponent<PopupBase>();
            if (pb)
            {
                pb.OpenPopup();
                pb.Closed += OnPopupClosed;
            }
            return obj;
        }
        void OnPopupClosed(GameObject obj)
        {
            PopupList.Remove(obj);
            UpdateIt();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (BlockClose) return;

                if (PopupList.Count > 0)
                {
                    CloseLastPopup();
                }
                else
                {
                    ShowOkPopup(ExitMessage, () =>
                    {
                        Application.Quit();
                    });
                }
            }
        }
        public static void ShowOkPopup(string message, UnityAction onOk)
        {
            GameObject obj = Instantiate(Resources.Load<GameObject>("Prefab/UI/Popup/pnlOk"), PopupManager.Instance.GetNextParent());
            obj.name = "pnlOk";
            obj.GetComponent<PopupBase>().OpenPopup();
        }

        static List<Transform> _previousMessages = new List<Transform>();
        static string _lastMsg;
        static Transform TopCanvas;
        public static void ShowToast(string text)
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
            string value = text;
            string messageName = "instantMessage";

            float moveY = 50;
            foreach (var item in _previousMessages)
            {
                InstantMessageUI msg = item.GetComponent<InstantMessageUI>();
                msg.StartY += moveY;
                msg.ShowY += moveY;
                item.localPosition += new Vector3(0, moveY);
            }
            Transform pnl = Instantiate(Resources.Load<GameObject>("Prefab/UI/ToastMessage"), GetTopCanvas()).transform;
            pnl.name = messageName;
            pnl.gameObject.SetActive(true);
            pnl.GetComponent<InstantMessageUI>().ShowMessage(value);
            _previousMessages.Add(pnl);
        }
        public static Transform GetTopCanvas()
        {
            if (!TopCanvas)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefab/UI/PopupCanvas");
                GameObject topCanvas = Instantiate(prefab);
                topCanvas.name = "TopCanvas";
                TopCanvas = topCanvas.transform;
            }
            TopCanvas.GetComponentInParent<Canvas>().sortingOrder = Instance.CanvasList.Count + 1;
            return TopCanvas;
        }
        public void CloseLastPopup()
        {
            if (PopupList.Count == 0) return;
            GameObject last = PopupList[PopupList.Count - 1];
            if (last.GetComponent<PopupBase>().BlockClose)
            {
                return;
            }
            ClosePopup(last.name);
        }
        public bool IsPopupOpen(string popup)
        {
            foreach (var item in PopupList)
            {
                if (item.name.Equals(popup)) return true;
            }
            return false;
        }
        public void OpenPopup(string popup)
        {
            if (IsHold) return;

            InstantiatePopup(popup);
        }
        public GameObject GetPopup(string popup)
        {
            foreach (var item in PopupList)
            {
                if (item.name.Equals(popup))
                {
                    return item;
                }
            }
            return null;
        }
        public void ClosePopup(string popup)
        {
            ClosePopupAndDestroy(GetPopup(popup));
        }

        void ClosePopupAndDestroy(GameObject obj)
        {
            StartCoroutine(DestroyLater(obj));
        }
        IEnumerator DestroyLater(GameObject obj)
        {
            obj.GetComponent<PopupBase>().ClosePopup();
            yield return new WaitForSeconds(1);
            Destroy(obj);
        }
        public void AddPopup(GameObject obj)
        {
            PopupList.Add(obj);
            UpdateIt();
            if (PopupList.Count > 1)
            {
                UpdateIt();
            }
        }
        public void RemovePopup(GameObject obj)
        {
            PopupList.Remove(obj);
            UpdateIt();
        }
        void CheckCam()
        {
            if (!MainCam || !UICam)
            {
                MainCam = Camera.main;
                Camera objCam = Resources.Load<Camera>("Prefab/BlurCamera");
                if (!objCam) return;
                Camera cam = Instantiate(objCam);
                if (!cam) return;
                cam.name = "BlurCamera";
                UICam = cam;
                UICam.transform.localPosition = MainCam.transform.position;
                UICam.transform.localRotation = MainCam.transform.rotation;
                UICam.transform.localScale = MainCam.transform.localScale;
            }
        }
        void UpdateIt()
        {
            CheckCam();

            int index = 0;
            foreach (var canvas in CanvasList)
            {
                Canvas can = canvas.GetComponent<Canvas>();
                can.renderMode = PopupList.Count > index + 1 ? RenderMode.ScreenSpaceCamera : RenderMode.ScreenSpaceOverlay;
                can.sortingOrder = index + 1;
                index++;
            }

            MainCam.GetComponent<Camera>().enabled = PopupList.Count <= 0;
            MainCam.GetComponent<AudioListener>().enabled = PopupList.Count <= 0;
            UICam.gameObject.SetActive(PopupList.Count > 0);
            UICam.transform.position = MainCam.transform.position;
            UICam.GetComponent<Camera>().enabled = PopupList.Count > 0;
            if (!MainCanvas)
            {
                MainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            }
            MainCanvas.renderMode = PopupList.Count > 0 ? RenderMode.ScreenSpaceCamera : RenderMode.ScreenSpaceOverlay;
            MainCanvas.worldCamera = UICam;
            MainCanvas.planeDistance = 1;
        }
    }
}
