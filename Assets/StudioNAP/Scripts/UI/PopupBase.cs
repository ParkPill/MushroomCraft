using System;
using System.Collections;
using System.Collections.Generic;
//using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StudioNAP
{
    public class PopupBase : MonoBehaviour
    {
        public bool BlockClose = false;
        protected Transform _content;
        public UnityAction<GameObject> Closed;
        public bool DestoryAfterClose = true;
        public bool AnimationEnabled = true;
        public bool OpenAnimated = true;
        // Start is called before the first frame update
        void Start()
        {
            Init();
        }
        protected virtual void OnClosed(GameObject obj)
        {

        }
        /// <summary>
        /// Initialize the popup
        /// </summary>
        public void Init()
        {
            if (_content == null)
            {
                Closed += OnClosed;
                Transform btnBlack = transform.Find("btnBlack");
                if (btnBlack && btnBlack.GetComponent<Image>().color.a < 1 && OpenAnimated)
                {
                    Animator ani = transform.GetComponent<Animator>();
                    if (!ani)
                    {
                        ani = gameObject.AddComponent<Animator>();
                        ani.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("UI/SmallPopup");
                    }

                    ani.Play(0, -1, 0);
                }

                _content = transform.Find("Content");
                Transform btnBack = transform.Find("btnBack");
                if (btnBack)
                {
                    btnBack.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ClosePopup();
                    });
                }

                Transform btnClose = _content.Find("btnClose");
                if (!btnClose)
                {
                    btnClose = transform.Find("btnClose");
                }

                if (btnClose)
                {
                    btnClose.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ClosePopup();
                    });
                }
            }
        }
        void ApplySafeArea()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                var Myrect = transform.GetComponent<RectTransform>();

                Vector2 minAnchor = Screen.safeArea.min;
                Vector2 maxAnchor = Screen.safeArea.max;

                minAnchor.x /= Screen.width;
                minAnchor.y /= Screen.height;

                maxAnchor.x /= Screen.width;
                maxAnchor.y /= Screen.height;


                Myrect.anchorMin = minAnchor;
                Myrect.anchorMax = maxAnchor;
            }
        }
        public Text GetText(string nodeName)
        {
            Init();
            Transform result = GetNode(nodeName);
            if (result) return result.GetComponent<Text>();

            return null;
        }
        public Image GetImage(string nodeName)
        {
            Init();
            Transform result = GetNode(nodeName);
            if (result) return result.GetComponent<Image>();

            return null;
        }
        public void ChangeImageColorAlpha(string nodeName, float alpha)
        {
            Image img = GetImage(nodeName);
            Color color = img.color;
            img.color = new Color(color.r, color.g, color.b, alpha);
            img.color = color;
        }
        public Button GetButton(string nodeName)
        {
            Init();
            return _content.Find(nodeName).GetComponent<Button>();
        }
        public Transform GetNode(string nodeName)
        {
            if (!_content) Init();
            return GetNode(nodeName, _content);
        }
        public Transform GetNode(string nodeName, Transform parent)
        {
            Transform result = parent.Find(nodeName);
            if (result) return result;

            foreach (Transform child in parent)
            {
                if (child.childCount > 0)
                {
                    result = GetNode(nodeName, child);
                    if (result) return result;
                }
            }
            return null;
        }
        public void OpenPopup(string popup)
        {
            PopupManager.Instance.OpenPopup(popup);
        }
        public void OpenPopup()
        {
            this.gameObject.SetActive(true);
            Vector3 scale = Vector3.one;
            Transform content = transform.Find("Content");
            if (content == null)
            {
                content = transform;
            }
            if (AnimationEnabled && false)
            {
                scale.x = 0.01f;
                content.localScale = scale;
                //content.DOScale(1, 0.5f).SetEase(Ease.OutExpo);
                StartCoroutine(DOScale(content, 1, 0.3f));
            }

            PopupManager.Instance.AddPopup(gameObject);
        }

        public void ClosePopup()
        {
            if (BlockClose)
            {
                return;
            }
            Closed?.Invoke(gameObject);
            Vector3 scale = new Vector3(0, 1, 1);

            Transform content = transform.Find("Content");
            if (content == null)
            {
                content = transform;
            }
            PopupManager.Instance.RemovePopup(gameObject);

            if (AnimationEnabled && false)
            {
                if (gameObject.activeSelf)
                {

                    StartCoroutine(DOScale(content, 0, 1, 1, 0.3f, () =>
                    {
                        if (DestoryAfterClose) Destroy(gameObject);
                        else gameObject.SetActive(false);
                    }));
                }
            }
            else
            {
                if (DestoryAfterClose) Destroy(gameObject);
                else gameObject.SetActive(false);
            }
        }
        public static IEnumerator DOScale(Transform transform, float scaleX, float scaleY, float scaleZ, float duration, Action onComplete = null)
        {
            return DOScale(transform, new Vector3(scaleX, scaleY, scaleZ), duration, onComplete);
        }
        public static IEnumerator DOScale(Transform transform, float scale, float duration, Action onComplete = null)
        {
            return DOScale(transform, new Vector3(scale, scale, scale), duration, onComplete);
        }
        public static IEnumerator DOScale(Transform transform, Vector3 targetScale, float duration, Action onComplete = null)
        {
            Vector3 startScale = transform.localScale;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                t = 1 - Mathf.Pow(1 - t, 4); // OutQuart 보간

                transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localScale = targetScale;


            if (onComplete != null)
            {
                onComplete();
            }
        }
        public void OnLinkClick(string url)
        {
            Application.OpenURL(url);
        }
    }
}
