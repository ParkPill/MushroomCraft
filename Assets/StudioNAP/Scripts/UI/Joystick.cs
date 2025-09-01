using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class Vector2Event : UnityEvent<Vector2> { }
[RequireComponent(typeof(RectTransform))]
public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform backgroundRect = null;
    [SerializeField] private RectTransform handleRect = null;
    [SerializeField] private float maxHandleDistance = 50f;

    private Vector2 inputDirection = Vector2.zero;
    private Vector2 direction = Vector2.zero;
    public Vector2Event OnValueChanged;
    public UnityAction<bool> Pressed;
    //public Transform Wheel;
    [HideInInspector]
    public Vector2 StartKnobPos;
    [HideInInspector]
    public Vector2 PressKnobPos;
    public bool FixPosition = false;
    public bool StayOn = false;
    public bool IsPressed { get; private set; }
    Color _startWheelColor;
    Color _startKnobColor;

    private void Start()
    {
        StartKnobPos = handleRect.transform.position;
        _startWheelColor = backgroundRect.GetComponent<Image>().color;
        _startKnobColor = handleRect.GetComponent<Image>().color;
        if (!StayOn)
        {
            backgroundRect.GetComponent<Image>().color = new Color(_startWheelColor.r, _startWheelColor.g, _startWheelColor.b, 0);
            handleRect.GetComponent<Image>().color = new Color(_startKnobColor.r, _startKnobColor.g, _startKnobColor.b, 0);
        }
    }

    public Vector2 GetDirection()
    {
        return direction;
    }

    public void OnDrag(PointerEventData eventData)
    {
        IsPressed = true;
        Vector2 pointerPosition = eventData.position;
        Vector2 backgroundPosition = backgroundRect.position;
        Vector2 offset = pointerPosition - backgroundPosition;
        float maxDistance = backgroundRect.rect.width / 2f;
        Vector2 direction = Vector2.ClampMagnitude(offset, maxDistance) / maxDistance;
        inputDirection = direction;
        direction = inputDirection.magnitude > 0.1f ? inputDirection : direction;
        handleRect.position = backgroundPosition + direction * maxHandleDistance;///transform.parent.localScale;
        //print("joystick value: " + direction);
        if (OnValueChanged != null) OnValueChanged.Invoke(direction);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!FixPosition)
        {
            handleRect.position = eventData.position;
            backgroundRect.position = eventData.position;
        }
        OnDrag(eventData);
        Pressed?.Invoke(true);
        IsPressed = true;
        backgroundRect.GetComponent<Image>().color = _startWheelColor;
        handleRect.GetComponent<Image>().color = _startKnobColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handleRect.position = StartKnobPos;
        backgroundRect.position = StartKnobPos;

        inputDirection = Vector2.zero;
        direction = Vector2.zero;
        //handleRect.anchoredPosition = Vector2.zero;
        OnValueChanged?.Invoke(direction);
        Pressed?.Invoke(false);
        IsPressed = false;

        if (!StayOn)
        {
            backgroundRect.GetComponent<Image>().color = new Color(_startWheelColor.r, _startWheelColor.g, _startWheelColor.b, 0);
            handleRect.GetComponent<Image>().color = new Color(_startKnobColor.r, _startKnobColor.g, _startKnobColor.b, 0);
        }
    }
}
