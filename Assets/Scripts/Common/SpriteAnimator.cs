using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    [Header("스프라이트 애니메이션 설정")]
    [SerializeField] private Sprite[] sprites;           // 애니메이션에 사용할 스프라이트 배열
    [SerializeField] private float interval = 0.1f;      // 프레임 간격 (초)
    [SerializeField] private bool playOnStart = true;    // 시작 시 자동 재생 여부
    [SerializeField] private bool loop = true;           // 반복 재생 여부

    private SpriteRenderer spriteRenderer;                // 스프라이트 렌더러 컴포넌트
    private int currentIndex = 0;                        // 현재 스프라이트 인덱스
    private float timer = 0f;                            // 타이머
    private bool isPlaying = false;                      // 재생 중인지 여부

    void Start()
    {
        // 스프라이트 렌더러 컴포넌트 가져오기
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 스프라이트가 없으면 경고 출력
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogWarning("SpriteAnimator: sprites 배열이 비어있습니다!", this);
            return;
        }

        // 첫 번째 스프라이트로 초기화
        if (spriteRenderer != null && sprites.Length > 0)
        {
            spriteRenderer.sprite = sprites[0];
        }

        // 시작 시 자동 재생
        // if (playOnStart)
        // {
        //     Play();
        // }
    }

    void OnEnable()
    {
        if (playOnStart)
        {
            Play();
        }
    }

    void Update()
    {
        if (!isPlaying || sprites == null || sprites.Length == 0) return;

        // 타이머 업데이트
        timer += Time.deltaTime;

        // 간격에 도달하면 다음 스프라이트로 변경
        if (timer >= interval)
        {
            timer = 0f;
            NextSprite();
        }
    }

    /// <summary>
    /// 다음 스프라이트로 변경
    /// </summary>
    private void NextSprite()
    {
        currentIndex++;

        // 마지막 스프라이트에 도달했을 때
        if (currentIndex >= sprites.Length)
        {
            if (loop)
            {
                // 반복 재생이면 처음부터 다시 시작
                currentIndex = 0;
            }
            else
            {
                // 반복 재생이 아니면 정지
                Stop();
                return;
            }
        }

        // 스프라이트 변경
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprites[currentIndex];
        }
    }

    /// <summary>
    /// 애니메이션 재생 시작
    /// </summary>
    public void Play()
    {
        if (sprites == null || sprites.Length == 0) return;

        isPlaying = true;
        currentIndex = 0;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprites[currentIndex];
        }
        timer = 0f;
    }

    /// <summary>
    /// 애니메이션 정지
    /// </summary>
    public void Stop()
    {
        isPlaying = false;
        timer = 0f;
    }

    /// <summary>
    /// 애니메이션 일시정지
    /// </summary>
    public void Pause()
    {
        isPlaying = false;
    }

    /// <summary>
    /// 애니메이션 재개
    /// </summary>
    public void Resume()
    {
        if (sprites == null || sprites.Length == 0) return;
        isPlaying = true;
    }

    /// <summary>
    /// 특정 인덱스의 스프라이트로 즉시 변경
    /// </summary>
    /// <param name="index">스프라이트 인덱스</param>
    public void SetSprite(int index)
    {
        if (sprites == null || sprites.Length == 0) return;
        if (index < 0 || index >= sprites.Length) return;

        currentIndex = index;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprites[currentIndex];
        }
    }

    /// <summary>
    /// 애니메이션 속도 설정 (간격 조절)
    /// </summary>
    /// <param name="newInterval">새로운 간격 (초)</param>
    public void SetInterval(float newInterval)
    {
        interval = Mathf.Max(0.01f, newInterval); // 최소 0.01초
    }

    /// <summary>
    /// 현재 재생 상태 반환
    /// </summary>
    /// <returns>재생 중이면 true</returns>
    public bool IsPlaying()
    {
        return isPlaying;
    }

    /// <summary>
    /// 현재 스프라이트 인덱스 반환
    /// </summary>
    /// <returns>현재 스프라이트 인덱스</returns>
    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    /// <summary>
    /// 총 스프라이트 개수 반환
    /// </summary>
    /// <returns>스프라이트 배열의 길이</returns>
    public int GetSpriteCount()
    {
        return sprites != null ? sprites.Length : 0;
    }
}
