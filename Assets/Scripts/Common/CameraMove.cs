using System;
using System.Collections.Generic;
using UnityEngine;

namespace StudioNAP
{
    public class CameraMove : MonoBehaviour
    {
        [Serializable]
        public class MoveData
        {
            public string Name;
            public Transform Target;
            public Vector3 Position;
            public Vector3 Offset;
            public Quaternion Rotation;
            public float Pinch = 66;
            public int Zoom;
        }

        [SerializeField]
        public MoveData[] MoveDataList;
        public int CurrentIndex = -1;

        public Vector3 LeftTopLimit = new Vector3(-38.02071f, 2.061394f, 0f);
        public Vector3 RightBottomLimit = new Vector3(34.72071f, -13.58f, 0f);

        public float moveSpeed = 2f;
        public float rotationSpeed = 0.1f;
        Camera _cam;
        //public bool ApplyStartCamPos = true;
        public bool UseSmooth = true;
        bool _isTargetSet = false;

        public string ActivatedName = "";

        private void Start()
        {
            //if (CurrentOffsetIndex == 0 && offsetList.Count == 0)
            //{
            //    offsetList.Add(offset);
            //}
            _cam = GetComponent<Camera>();
            //if (_cam != null && ApplyStartCamPos)
            //{
            //    Vector3 topLeft = _cam.ScreenToWorldPoint(new Vector3(0, _cam.pixelHeight, _cam.nearClipPlane));
            //    Vector3 current = transform.position;
            //    float x = current.x - topLeft.x;
            //    float y = topLeft.y - current.y;
            //    LTLimit += new Vector3(x, -y);
            //    RBLimit += new Vector3(-x, y);
            //}
        }
        //void SetCurrentIndex(int index)
        //{
        //    CurrentIndex = index;
        //    if (CurrentIndex < 0)
        //    {
        //        return;
        //    }


        //    MoveData moveData = MoveDataList[CurrentIndex];
            
        //}
        private void Update()
        {
            if (CurrentIndex < 0)
            {
                return;
            }
            Vector3 pos;
            MoveData moveData = MoveDataList[CurrentIndex];
            if (moveData.Target != null)
            {
                if (!_isTargetSet) _isTargetSet = true;
                pos = moveData.Target.position;
            }
            else
            {
                pos = moveData.Position;
            }
            Vector3 selectedOffset = moveData.Offset;

            Vector3 velo = new Vector3(0, 0, 0);
            Vector3 currentPinch = new Vector3(_cam.fieldOfView, 0, 0);
            Vector3 targetPinch = new Vector3(moveData.Pinch, 0, 0);
            _cam.fieldOfView = Vector3.SmoothDamp(currentPinch, targetPinch, ref velo, rotationSpeed).x; 

            pos += selectedOffset;

            if (LeftTopLimit != default)
            {
                if (pos.x < LeftTopLimit.x) pos.x = LeftTopLimit.x;
                if (pos.x > RightBottomLimit.x) pos.x = RightBottomLimit.x;
                if (pos.y < RightBottomLimit.y) pos.y = RightBottomLimit.y;
                if (pos.y > LeftTopLimit.y) pos.y = LeftTopLimit.y;
            }

            if (UseSmooth)
            {
                transform.position = Vector3.SmoothDamp(transform.position, pos, ref velo, 1f/moveSpeed);
                _cam.transform.rotation = Quaternion.Lerp(transform.rotation, moveData.Rotation, rotationSpeed);
            }
            else
            {
                transform.position = pos;
                _cam.transform.rotation = moveData.Rotation;
            }

        }

        public void RunTest()
        {
            MoveData mData = null;
            int index = 0;
            foreach (var item in MoveDataList)
            {
                if (item.Name.Equals(ActivatedName))
                {
                    mData = item;
                    break;
                }
                index++;
            }

            if (mData != null)
            {
                CurrentIndex = index;
            }
        }
        public void AddCamData()
        {
            MoveData mData = new MoveData();
            Transform cam = Camera.main.transform;
            mData.Position = cam.position;
            mData.Rotation = cam.rotation;
            mData.Pinch = Camera.main.fieldOfView;
            List<MoveData> list = new List<MoveData>(MoveDataList);
            list.Add(mData);
            MoveDataList = list.ToArray();
        }
    }

}