using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationHandler : MonoBehaviour
{
    public UnityAction OnAttack;
    public void OnAttackEvent()
    {
        OnAttack?.Invoke();
    }
}
