using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSec : MonoBehaviour
{
    public bool PlayOnAwake = false;
    public float Dur = 2;
    bool _isTimeSet = false;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayOnAwake)
        {
            DestroyAfter(Dur);
        }
    }
    private void Update()
    {
        if (_isTimeSet)
        {
            Dur -= Time.deltaTime;
            if (Dur < 0) Destroy(gameObject);
        }
    }
    public void DestroyAfter(float dur)
    {
        Dur = dur;
        _isTimeSet = true;
        //StartCoroutine(DestroyLater(dur));
    }
    //IEnumerator DestroyLater(float dur)
    //{
    //    yield return new WaitForSeconds(dur);
    //    Destroy(gameObject);
    //}
}
