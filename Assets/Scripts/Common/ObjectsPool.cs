using System.Collections.Generic;
using UnityEngine;

public class ObjectsPool : PoolBase
{
    public List<GameObject> prefabList;
    [SerializeField] private int _limit = 3;

    private Dictionary<string, List<GameObject>> _pools;
    private Dictionary<string, int> _indices;

    private void Start()
    {
        InitializePools();
    }

    private void InitializePools()
    {
        if (_pools == null)
        {
            _pools = new Dictionary<string, List<GameObject>>();
            _indices = new Dictionary<string, int>();
        }
    }

    public void Clear()
    {
        if (_pools != null)
        {
            foreach (var pool in _pools.Values)
            {
                foreach (var obj in pool)
                {
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
                pool.Clear();
            }
            _pools.Clear();
        }
        _indices.Clear();
    }

    public void AddObject(GameObject obj)
    {
        prefabList.Add(obj);
    }

    public GameObject GetObject(string objName)
    {
        InitializePools();

        // 프리팹 찾기
        GameObject prefab = prefabList.Find(x => x && x.name.Equals(objName));
        if (prefab == null) return null;

        // 해당 이름의 풀이 없으면 생성
        if (!_pools.ContainsKey(objName))
        {
            _pools[objName] = new List<GameObject>();
            _indices[objName] = 0;
        }

        var pool = _pools[objName];
        GameObject obj = null;

        // 먼저 비활성화된 오브젝트 찾기
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i] != null && !pool[i].activeInHierarchy)
            {
                obj = pool[i];
                break;
            }
        }

        // 비활성화된 오브젝트가 없는 경우
        if (obj == null)
        {
            // 풀 크기가 제한보다 작으면 새로 생성
            if (pool.Count < _limit)
            {
                obj = Instantiate(prefab, transform);
                obj.name = prefab.name;
                pool.Add(obj);
            }
            // 제한에 도달했으면 순환하여 재사용
            else
            {
                int index = _indices[objName];
                obj = pool[index];
                _indices[objName] = (index + 1) % _limit;
            }
        }

        // 위치와 회전 초기화
        obj.transform.position = transform.position;
        obj.transform.rotation = Quaternion.identity;
        if (obj.activeSelf)
        {
            ParticleSystem ps = obj.GetComponent<ParticleSystem>();
            if (ps)
            {
                ps.Stop();
                ps.Play();
            }
        }
        obj.SetActive(true);
        obj.transform.SetParent(transform);

        // ObjectPoolItem 컴포넌트 설정
        ObjectPoolItem poolItem = obj.GetComponent<ObjectPoolItem>();
        if (!poolItem) poolItem = obj.AddComponent<ObjectPoolItem>();
        poolItem.Parent = this;
        poolItem.StartTimer(poolItem.EnableTime);

        return obj;
    }

    public override void ReturnObject(GameObject obj)
    {
        if (obj == null) return;

        obj.SetActive(false);
        obj.transform.SetParent(transform);

        // 풀에 없으면 추가
        string objName = obj.name;
        if (!_pools.ContainsKey(objName))
        {
            _pools[objName] = new List<GameObject>();
        }

        if (!_pools[objName].Contains(obj))
        {
            _pools[objName].Add(obj);
        }
    }
}
