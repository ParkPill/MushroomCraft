using System;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 0f, 0f);
    public List<Vector3> offsetList = new List<Vector3>();
    public int CurrentOffsetIndex = 0;
    public Vector3 LTLimit = new Vector3(0f, 0, 0f);
    public Vector3 RBLimit = new Vector3(0f, 0, 0f);
    public float followSpeed = 0.1f;
    Camera _cam;
    public bool ApplyStartCamPos = true;
    public bool UseSmooth = true;
    bool _isTargetSet = false;
    public bool DestoryIfTargetIsNull = false;
    private void Start()
    {
        if (CurrentOffsetIndex == 0 && offsetList.Count == 0)
        {
            offsetList.Add(offset);
        }
        _cam = GetComponent<Camera>();
        if (_cam != null && ApplyStartCamPos)
        {
            Vector3 topLeft = _cam.ScreenToWorldPoint(new Vector3(0, _cam.pixelHeight, _cam.nearClipPlane));
            Vector3 current = transform.position;
            float x = current.x - topLeft.x;
            float y = topLeft.y - current.y;
            LTLimit += new Vector3(x, -y);
            RBLimit += new Vector3(-x, y);
        }
    }
    public void SetOffsetIndex(int index)
    {
        CurrentOffsetIndex = index;
    }
    private void LateUpdate()
    {
        if (target != null)
        {
            if (!_isTargetSet) _isTargetSet = true;
            Vector3 selectedOffset = offsetList[CurrentOffsetIndex];
            Vector3 pos = target.position + selectedOffset;
            if (LTLimit != default)
            {
                if (pos.x < LTLimit.x) pos.x = LTLimit.x;
                if (pos.x > RBLimit.x) pos.x = RBLimit.x;
                if (pos.y < RBLimit.y) pos.y = RBLimit.y;
                if (pos.y > LTLimit.y) pos.y = LTLimit.y;
            }
            if (UseSmooth) transform.position = Vector3.Lerp(transform.position, pos, followSpeed);
            else transform.position = pos;
        }
        else
        {
            if (_isTargetSet)
            {
                ObjectPoolItem poolItem = GetComponent<ObjectPoolItem>();
                if (poolItem) poolItem.EndLife();

                if (DestoryIfTargetIsNull) Destroy(gameObject);
            }

        }
    }
}