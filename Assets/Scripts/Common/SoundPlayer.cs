using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundPlayer : MonoBehaviour
{
    public float Delay = 0;
    public bool PlayOnStart = true;
    bool _played = false;
    float _timer;
    public AudioClip soundClip;
    public List<AudioClip> soundClipList;
    public AudioMixerGroup mixerGroup; // SFX로 이름 지은 오디오 믹서
    public bool DestroyAfterPlay = false;

    private void Awake()
    {
        if (mixerGroup == null)
        {
            mixerGroup = Resources.Load<AudioMixerGroup>("Settings/SFX"); // 경로는 아래 설명 참고
            if (mixerGroup == null)
            {
                Debug.LogWarning("SFX == null");
            }
        }
    }

    public void Play()
    {
        _played = false;
        _timer = 0;
    }
    private void PlaySound()
    {
        _played = true;
        // 클릭 소리 재생
        if (soundClipList.Count > 0)
        {
            soundClip = soundClipList[Random.Range(0, soundClipList.Count)];
        }
        if (soundClip != null)
        {
            SoundPool pool = SoundPool.Instance;
            if (pool != null)
            {
                pool.PlaySound(soundClip, mixerGroup);
            }
            else
            {
                GameObject soundObj = new GameObject("SoundPlayer");
                AudioSource audioSource = soundObj.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = mixerGroup; // SFX로 이름 지은 믹서 설정
                audioSource.clip = soundClip;
                audioSource.Play();
                if (DestroyAfterPlay) Destroy(soundObj, soundClip.length); // 소리 재생이 끝난 뒤에 임시 오브젝트 파괴
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnEnable()
    {
        //print("sound player enabled:  ");
        if (PlayOnStart) Play();
        else _timer = Delay + 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (_played) return;
        if (_timer <= Delay)
        {
            _timer += Time.deltaTime;
            if (_timer > Delay)
            {
                PlaySound();
            }
        }
    }
}
