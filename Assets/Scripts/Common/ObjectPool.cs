using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : PoolBase
{
    public GameObject prefab;
    public int poolSize = 10;
    int _index = 0;
    int _limit = 3;

    // private Queue<GameObject> pool;
    List<GameObject> pool = new List<GameObject>();

    private void Start()
    {
        CheckPool();

        for (int i = 0; i < poolSize; i++)
        {
            if (!prefab)
            {
                // print("object pool prefab is not set: " + name);
                continue;
            }
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }
    void CheckPool()
    {
        if (pool == null) pool = new List<GameObject>(poolSize);
    }

    public GameObject GetObject()
    {
        CheckPool();
        foreach (var item in pool)
        {
            if (!item.activeSelf)
            {
                item.SetActive(true);
                return item;
            }
        }
        if (pool.Count >= _limit)
        {
            GameObject forcedObj = pool[_index];
            forcedObj.SetActive(true);
            _index++;
            if (_index >= pool.Count) _index = 0;
            return forcedObj;
        }
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(true);
        pool.Add(obj);
        // GameObject obj;
        // if (pool.Count > 0)
        // {
        //     obj = pool[0];
        //     pool.RemoveAt(0);
        //     obj.SetActive(true);
        // }
        // else
        // {
        //     obj = Instantiate(prefab, transform);
        //     obj.SetActive(true);
        // }
        // obj.GetComponent<ObjectPoolItem>().Parent = this;
        return obj;
    }

    public override void ReturnObject(GameObject obj)
    {
        //print("return object");
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        // pool.Add(obj);
    }
}
