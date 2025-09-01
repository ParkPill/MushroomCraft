using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//[RequireComponent(typeof(Text))]
public class TextExposure : MonoBehaviour
{
    private Text _text;
    private TextMeshProUGUI _tmpUGUI;
    private float _timer;
    public float TextInterval = 0.07f;
    public string TextToShow;
    private int _indexToShow = 0;
    public bool IsDone = false;
    public bool IsStarted = false;
    public bool PlayOnStart = false;
    private UnityAction _callback;
    // Start is called before the first frame update
    void Start()
    {
        if (_text == null) _text = GetComponent<Text>();
        if (_tmpUGUI == null) _tmpUGUI = GetComponent<TextMeshProUGUI>();
        if (PlayOnStart)
        {
            StartTextRole(_text.text, null);
        }
    }
    public TextMeshProUGUI GetTMP()
    {
        return _tmpUGUI;
    }
    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        if (IsStarted)
        {
            _timer += dt;
            if (_timer > TextInterval)
            {
                _timer -= TextInterval;
                _indexToShow++;
                if (_indexToShow >= TextToShow.Length)
                {
                    OnTextRoleDone();
                    return;
                }

                if (_text) _text.text = TextToShow.Substring(0, _indexToShow);
                if (_tmpUGUI) _tmpUGUI.text = TextToShow.Substring(0, _indexToShow);
            }
        }
    }
    public void StartTextRole(string text)
    {
        StartTextRole(text, null);
    }
    public void StartTextRole(string text, UnityAction callback = null)
    {
        TextToShow = text;
        _callback = callback;
        StartTextRole();
    }
    public void StartTextRole()
    {
        if (_text == null) _text = GetComponent<Text>();
        if (_tmpUGUI) _tmpUGUI = GetComponent<TextMeshProUGUI>();
        IsDone = false;
        IsStarted = true;
        if (_text) _text.text = string.Empty;
        if (_tmpUGUI) _tmpUGUI.text = string.Empty;
        _indexToShow = 0;
        if (_text && _text.GetComponent<ContentSizeFitter>()) _coroutine = StartCoroutine(SetCenter());
        else if (_tmpUGUI && _tmpUGUI.GetComponent<ContentSizeFitter>()) _coroutine = StartCoroutine(SetCenter());
    }
    Coroutine _coroutine;
    IEnumerator SetCenter()
    {
        _timer = -0.4f;
        Color color = _text.color;
        if (_text) _text.color = new Color(1, 1, 1, 0);
        if (_tmpUGUI) _tmpUGUI.color = new Color(1, 1, 1, 0);
        if (_text)
        {
            ContentSizeFitter csf = _text.GetComponent<ContentSizeFitter>();
            csf.enabled = true;
        }
        if (_tmpUGUI)
        {
            ContentSizeFitter csf = _tmpUGUI.GetComponent<ContentSizeFitter>();
            csf.enabled = true;
        }
        if (_text) _text.text = TextToShow;
        if (_tmpUGUI) _tmpUGUI.text = TextToShow;
        yield return new WaitForSeconds(0.4f);
        if (_text) _text.text = string.Empty;
        if (_tmpUGUI) _tmpUGUI.text = string.Empty;
        if (_text) _text.color = new Color(color.r, color.g, color.b, 1);
        if (_tmpUGUI) _tmpUGUI.color = new Color(color.r, color.g, color.b, 1);

        // yield return new WaitForSeconds(0.4f);
        // csf.enabled = false;
        _coroutine = null;
    }
    public void Skip()
    {
        if (_coroutine != null) return;
        _indexToShow = TextToShow.Length;
        OnTextRoleDone();
    }
    public void OnTextRoleDone()
    {
        IsStarted = false;
        if (_text) _text.text = TextToShow;
        if (_tmpUGUI) _tmpUGUI.text = TextToShow;
        IsDone = true;

        _callback?.Invoke();
    }
}
