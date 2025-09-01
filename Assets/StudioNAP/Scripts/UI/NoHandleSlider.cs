using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NoHandleSlider : MonoBehaviour
{
    [Range(0, 1)] public float Value;
    private float _movingValue;
    private float _timer;
    public Image Img;
    public float Speed = 1;

    // Start is called before the first frame update
    void Start()
    {
        _movingValue = -1;
        if (Img == null)
        {
            Img = transform.Find("Image").GetComponent<Image>();
        }
    }


#if UNITY_EDITOR
    private bool _isProgressing = false;
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        if (Mathf.Approximately(_movingValue, Value)) return;

        Image img;
        if (Img) img = Img;
        else img = transform.Find("Image").GetComponent<Image>();

        RectTransform imgRect = img.GetComponent<RectTransform>();
        float parentWidth = ((RectTransform)imgRect.parent).rect.width;

        float targetWidth = parentWidth * Value;
        _movingValue = targetWidth;
        imgRect.sizeDelta = new Vector2(_movingValue, imgRect.sizeDelta.y);

    }
#endif

    void Update()
    {
        if (_movingValue != Value)
        {
            Image img;
            if (Img) img = Img;
            else img = transform.Find("Image").GetComponent<Image>();

            RectTransform imgRect = img.GetComponent<RectTransform>();
            float parentWidth = ((RectTransform)imgRect.parent).rect.width;

            float targetWidth = parentWidth * Value;
            float currentWidth = imgRect.sizeDelta.x;

            _movingValue = currentWidth - (currentWidth - targetWidth) * Speed / 40;
            imgRect.sizeDelta = new Vector2(_movingValue, imgRect.sizeDelta.y);

            if (Mathf.Abs(_movingValue - targetWidth) < 0.5f)
            {
                _movingValue = targetWidth;
                imgRect.sizeDelta = new Vector2(_movingValue, imgRect.sizeDelta.y);
            }
        }
    }

}
