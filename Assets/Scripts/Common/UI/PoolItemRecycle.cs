using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolItemRecycle : MonoBehaviour
{
    public static PoolItemRecycle Recycle(Transform content)
    {
        PoolItemRecycle recycle = content.GetComponent<PoolItemRecycle>();
        if (recycle == null)
        {
            recycle = content.gameObject.AddComponent<PoolItemRecycle>();
        }

        foreach (Transform child in content)
        {
            if (!child.name.Equals("temp") && child.gameObject.activeSelf)
            {
                child.name = "poolitem";
                child.gameObject.SetActive(false);
                recycle._poolItemList.Add(child);
            }
        }

        return recycle;
    }
    public List<Transform> _poolItemList = new List<Transform>();

    public Transform GetItem(GameObject temp)
    {
        return GetItem(temp.transform);
    }
    public Transform GetItem(Transform temp)
    {
        temp.gameObject.SetActive(false);
        if (_poolItemList.Count <= 0)
        {
            Transform newItem = Instantiate(temp.gameObject, temp.parent).transform;
            newItem.gameObject.SetActive(true);
            return newItem;
        }
        Transform item = _poolItemList[0];
        _poolItemList.RemoveAt(0);
        item.gameObject.SetActive(true);
        item.SetAsLastSibling();
        return item;
    }

    public T GetItem<T>(T prefab) where T : Component
    {
        Transform instance = GetItem(prefab.transform);
        return instance.GetComponent<T>();
    }

    //public void ReturnItem(Transform item)
    //{
    //    item.gameObject.SetActive(false);
    //    _poolItemList.Add(item);
    //}
}
