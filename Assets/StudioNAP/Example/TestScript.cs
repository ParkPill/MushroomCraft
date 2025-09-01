using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StudioNAP;

public class TestScript : MonoBehaviour
{
    public int ToastCount = 0;
    public Transform TestHero;
    public Vector2 JoystickDirection;
    public NoHandleSlider HP;
    public TextExposure TextExposure;
    public TimerUI Timer;
    public UINumberRaiser TheGold;
    public void OnToastClick()
    {
        PopupManager.ShowToast("Here is your simple message " + ToastCount);
        ToastCount++;
    }
    public void OnPopupBase()
    {
        PopupManager.Instance.OpenPopup("pnlPopupBase");
    }
    public void OnDialogClick()
    {
        PopupManager.Instance.Open<pnlDialog>("pnlDialog").ShowDialog("confirm purchase", "ask buy", () =>
        {
            PopupManager.ShowToast("buy success");
        });
    }

    public void OnJoystickClick(Vector2 direction)
    {
        JoystickDirection = direction;
    }
    void Update()
    {
        if (JoystickDirection != default)
        {
            TestHero.Translate(JoystickDirection * 0.01f);
        }
    }

    public void OnHPClick()
    {
        float value = Random.Range(0f, 1f);

        HP.Value = value;
    }

    public void OnTextExposureClick()
    {
        string text = "This is what TextExposure.cs script does.";

        TextExposure.StartTextRole(text);
    }

    public void OnTextExposureWithCallbackClick()
    {
        string text = "This is what TextExposure.cs script does.";


        TextExposure.StartTextRole(text, () =>
        {
            PopupManager.ShowToast("TextExposure done");
        });
    }

    public void OnTimerUIClick()
    {
        int sec = Random.Range(10, 90);

        Timer.SetTime(sec);
    }
    public void OnGoldGetClickWithAnimation()
    {
        Vector2 worldPos = TestHero.position;

        TheGold.ShowCurrencyGet(worldPos, 1000);
    }
    public void OnGoldGetClick()
    {
        TheGold.AddNumber(1000);
    }
}
