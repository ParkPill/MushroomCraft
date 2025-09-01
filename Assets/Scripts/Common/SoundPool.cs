using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundPool : PoolBase
{
    private static SoundPool _instance;
    public static SoundPool Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(SoundPool)) as SoundPool;
                if (!_instance)
                {
                    GameObject container = new GameObject();
                    container.name = "SoundPool";
                    _instance = container.AddComponent(typeof(SoundPool)) as SoundPool;
                    //container.AddComponent<AudioSource>();
                    DontDestroyOnLoad(_instance);
                }
            }
            return _instance;
        }
    }
    private List<GameObject> pool = new List<GameObject>();
    private List<AudioClip> clipList = new List<AudioClip>();

    private void Start()
    {
        _instance = this;
    }

    public void PlaySound(AudioClip clip, AudioMixerGroup mixer)
    {
        if (clip == null)
        {
            Debug.LogWarning("PlaySound: clip is null!");
            return;
        }

        GameObject obj = null;
        AudioSource source = null;

        // 풀에서 재사용 가능한 오브젝트 탐색
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].name == clip.name)
            {
                obj = pool[i];
                pool.RemoveAt(i);
                break;
            }
        }

        // 오브젝트가 없으면 새로 생성
        if (obj == null)
        {
            obj = new GameObject(clip.name);
            obj.transform.SetParent(transform);
            source = obj.AddComponent<AudioSource>();
            obj.AddComponent<ObjectPoolItem>();
        }
        else
        {
            obj.SetActive(true);
            source = obj.GetComponent<AudioSource>();
        }

        // 공통 AudioSource 세팅
        source.clip = clip;
        source.outputAudioMixerGroup = mixer;
        source.playOnAwake = false;

        // 타이머 세팅 후 재생
        ObjectPoolItem poolItem = obj.GetComponent<ObjectPoolItem>();
        poolItem.Parent = this;
        poolItem.StartTimer(clip.length);
        source.Play();
    }


    public override void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Add(obj);
    }
}
