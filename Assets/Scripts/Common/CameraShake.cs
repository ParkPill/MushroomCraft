using System.Collections;
using StudioNAP;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // 흔들림의 정도를 조절하는 변수
    public float shakeIntensity = 0.05f;
    // Coroutine 실행 시간
    public float shakeTime = 0.1f;
    public int shakeDistance = 50;
    private float shakeDelay = 0;
    // FollowTarget 스크립트 컴포넌트
    private FollowTarget followTarget;
    public bool ControlFollowTarget = true;
    CameraMove cameraMove;
    bool _useCameraMove = true;
    bool _shaking = false;
    Coroutine coroutine;
    Vector3 _initialPosition;
    Vector3 _startPosition;
    public bool AlwaysBackToStartPosition = false;
    public bool ForceTurnOFF = false;
    void Start()
    {
        followTarget = GetComponent<FollowTarget>();
        _startPosition = transform.position;
    }
    // Coroutine 함수
    IEnumerator ShakeCoroutine(Vector3 shakeDirection)
    {
        _shaking = true;
        _initialPosition = transform.position;
        // FollowTarget 스크립트를 일시적으로 비활성화
        //bool isFollowEnabledBefore = false;
        bool targetMoveHandled = false;
        if (ControlFollowTarget)
        {
            if (followTarget)
            {
                followTarget.enabled = false;
                targetMoveHandled = true;
            }
            else if (cameraMove)
            {
                cameraMove.enabled = false;
                targetMoveHandled = true;
            }
        }

        yield return new WaitForSeconds(shakeDelay);
        // 초기 카메라 위치를 저장
        Vector3 cameraInitialPosition = transform.position;

        // Mathf.Sin 함수를 사용하여 카메라를 일정한 간격으로 이동시킴
        float elapsedTime = 0f;
        while (elapsedTime < shakeTime)
        {
            cameraInitialPosition = transform.position;
            // print("cameraInitialPosition: " + cameraInitialPosition);
            float shakeAmount = Mathf.Sin(Time.time * shakeDistance) * shakeIntensity;
            Vector3 newPos = cameraInitialPosition + shakeDirection * shakeAmount;
            transform.position = newPos;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 초기 카메라 위치로 되돌림
        if (!targetMoveHandled) transform.position = cameraInitialPosition;

        // FollowTarget 스크립트를 다시 활성화
        if (ControlFollowTarget)
        {
            if (followTarget) followTarget.enabled = true;
            else if (cameraMove) cameraMove.enabled = true;
        }
        _shaking = false;
        if (AlwaysBackToStartPosition) transform.position = _startPosition;
        else transform.position = _initialPosition;
    }

    // 외부에서 호출할 함수
    public void Shake(float dur = 0.1f, float delay = 0)
    {
        if (ForceTurnOFF) return;
        // print("Shake: " + dur + " " + delay);
        bool isShake = PlayerPrefs.GetInt(Consts.Key_ShakeOn, 1) == 1;
        if (!isShake) return;
        if (_shaking && coroutine != null) StopCoroutine(coroutine);

        shakeTime = dur;
        shakeDelay = delay;

        if (!gameObject.activeInHierarchy) return;
        // 랜덤한 방향 벡터 생성
        Vector3 shakeDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
        //print("shake!");
        // Coroutine을 실행하여 특정 시간 동안만 카메라를 흔들림
        coroutine = StartCoroutine(ShakeCoroutine(shakeDirection));
    }
    public void ShakeHalfSec()
    {
        Shake(.5f);
    }
    public void ShakeOneSec()
    {
        Shake(1);
    }
    public void ShakeTwoSec()
    {
        Shake(2);
    }
}
