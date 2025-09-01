using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StudioNAP
{
    public class UINumberRaiser : MonoBehaviour
    {
        private bool IsTextMesh = false;
        private TextMeshProUGUI _lblMesh;
        private Text _lbl;
        private float _timeChecker = 0;
        //public float RaiseInterval = 0.2f;
        public float RaiseDur = 0.5f;
        public BigNum Number;
        private BigNum _previousNumber = -1;
        public string Prefix;
        public string Postfix;
        public Sprite CurrencyIcon;

        // 코인 애니메이션 관련 변수들
        private List<GameObject> coinObjects = new List<GameObject>();
        private bool isAnimatingCoins = false;
        private int currentCoinIndex = 0;
        private int targetNumber = 0;
        private int originalNumber = 0;
        public float CurrencySpreadRadius = 100f;
        public Vector2 CurrencySize = new Vector2(50f, 50f);
        public int CurrencyCount = 7;
        bool _holdNumber = false;
        private float _animationIntervalChecker = 0;
        float _animationInterval = 0.07f;

        // Start is called before the first frame update
        void Start()
        {
            SetUI();
            IsTextMesh = _lblMesh != null;
        }
        void SetUI()
        {
            _lblMesh = GetComponent<TextMeshProUGUI>();
            _lbl = GetComponent<Text>();
        }
        public void Invalidate()
        {
            _previousNumber = Number - 1.0;
        }
        // Update is called once per frame
        void Update()
        {
            if (_previousNumber == Number)
            {
                SetNumber(Number);
                return;
            }
            if (_holdNumber)
            {
                return;
            }
            float dt = Time.deltaTime;
            _timeChecker += dt;
            if (_timeChecker >= RaiseDur)
            {
                _timeChecker = 0;
                _previousNumber = Number;
            }
            _animationIntervalChecker += dt;
            if (_animationIntervalChecker >= _animationInterval)
            {
                _animationIntervalChecker = 0;

                if (_previousNumber < Number)
                {
                    _previousNumber += ((Number - _previousNumber) * _animationInterval);
                    if (_previousNumber > Number)
                    {
                        _previousNumber = Number;
                    }
                }
                else if (_previousNumber > Number)
                {
                    _previousNumber -= ((_previousNumber - Number) * _animationInterval);
                    if (_previousNumber < Number)
                    {
                        _previousNumber = Number;
                    }
                }
                if (IsTextMesh)
                {
                    _lblMesh.text = string.Format("{0}{1}{2}", Prefix, _previousNumber.ToStringFiveChars(), Postfix);
                }
                else
                {
                    _lbl.text = string.Format("{0}{1}{2}", Prefix, _previousNumber.ToStringFiveChars(), Postfix);
                }
            }
        }
        public int GetNumber(string text)
        {
            BigNum num = new BigNum(text);
            return (int)num.ToNum();

            //int multiply = 1;
            //if (text.EndsWith("K"))
            //{
            //    multiply *= 1000;
            //}
            //else if (text.EndsWith("M"))
            //{
            //    multiply *= 1000000;
            //}
            //else if (text.EndsWith("B"))
            //{
            //    multiply *= 1000000000;
            //}
            //int number = 0;
            //if (multiply == 1)
            //{
            //    number = int.Parse(text); 
            //}
            //else
            //{
            //    number = int.Parse(text.Substring(0, text.Length - 1));
            //}
            //return number*multiply;
        }
        public void SetNumber(int num)
        {
            SetNumber(new BigNum(num));
        }
        public void AddNumber(int num)
        {
            SetNumber(Number + num);
        }
        public void SetNumber(BigNum num)
        {
            if (_lbl == null && _lblMesh == null)
            {
                _lblMesh = GetComponent<TextMeshProUGUI>();
                _lbl = GetComponent<Text>();
            }
            _previousNumber = Number;
            Number = num;
            if (IsTextMesh)
            {
                _lblMesh.text = string.Format("{0}{1}{2}", Prefix, num, Postfix);
            }
            else
            {
                _lbl.text = string.Format("{0}{1}{2}", Prefix, num, Postfix);
            }
        }
        public string GetFormattedNumber(int num)
        {
            BigNum bn = new BigNum(num);

            return bn.ToStringFiveChars();
        }

        public void ShowCurrencyGet(Vector3 startPosInWorld, int num)
        {
            if (isAnimatingCoins) return; // 이미 애니메이션 중이면 무시

            // 기존 코인 오브젝트들 정리
            ClearCoinObjects();

            isAnimatingCoins = true;
            currentCoinIndex = 0;
            originalNumber = (int)Number.ToNum();
            targetNumber = originalNumber + num;
            Number = new BigNum(targetNumber);

            // 숫자 초기화 (애니메이션 시작 전)
            _previousNumber = originalNumber;
            _timeChecker = 0;
            _holdNumber = true;
            // 코인 오브젝트들 생성
            CreateCoinObjects(startPosInWorld);

            // 코인 애니메이션 시작
            StartCoroutine(AnimateCoins());
        }

        private void CreateCoinObjects(Vector3 startPosInWorld)
        {
            int coinCount = CurrencyCount;
            float spreadRadius = CurrencySpreadRadius; // 코인들이 퍼지는 반지름

            Canvas rootCanvas = GetComponentInParent<Canvas>().rootCanvas;

            for (int i = 0; i < coinCount; i++)
            {
                // 코인 오브젝트 생성
                GameObject coinObj = new GameObject("Coin_" + i);
                coinObj.transform.SetParent(rootCanvas.transform); // 캔버스의 자식으로 설정

                // 이미지 컴포넌트 추가
                Image coinImage = coinObj.AddComponent<Image>();
                coinImage.sprite = CurrencyIcon;
                coinImage.raycastTarget = false; // 클릭 이벤트 방지

                // 코인 크기 설정
                RectTransform rectTransform = coinObj.GetComponent<RectTransform>();
                rectTransform.sizeDelta = CurrencySize; // 작은 크기

                // 시작 위치 설정 (중앙에서 시작)
                Vector2 canvasPos = WorldToCanvasPosition(startPosInWorld, rootCanvas);
                rectTransform.anchoredPosition = canvasPos;

                // 초기 크기를 0으로 설정 (퍼지는 애니메이션을 위해)
                rectTransform.localScale = Vector3.zero;

                coinObjects.Add(coinObj);
            }
        }

        private Vector2 WorldToCanvasPosition(Vector3 worldPosition, Canvas canvas)
        {
            // Screen Space - Overlay 캔버스의 경우 카메라 없이 스크린 좌표를 직접 사용
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

            // 스크린 좌표를 캔버스 좌표로 변환
            Vector2 canvasPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos,
                null, // Screen Space - Overlay에서는 카메라가 null
                out canvasPos
            );

            return canvasPos;
        }

        private IEnumerator AnimateCoins()
        {
            // UINumberRaiser의 월드 포지션을 캔버스 좌표로 변환
            Canvas rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
            int loopCount = 0;
            Vector3 screenOffset = transform.localPosition;
            Transform parent = transform.parent;
            while (parent != rootCanvas.transform)
            {
                loopCount++;
                screenOffset += parent.localPosition;
                parent = parent.parent;
                if (loopCount > 20)
                {
                    break;
                }
            }

            Vector2 targetPos = screenOffset;

            float spreadDuration = 0.2f; // 퍼지는 애니메이션 시간
            float flyDuration = 0.7f; // 날아가는 애니메이션 시간
            float delayBetweenCoins = 0.05f;

            // 먼저 모든 코인을 퍼지게 함
            yield return StartCoroutine(SpreadCoins(spreadDuration));

            yield return new WaitForSeconds(spreadDuration * 0.5f);
            StartCoroutine(StartNumberAnimation(flyDuration));

            // 각 코인을 순차적으로 날아가게 함
            for (int i = 0; i < coinObjects.Count; i++)
            {
                StartCoroutine(AnimateSingleCoin(coinObjects[i], targetPos, flyDuration));

                yield return new WaitForSeconds(delayBetweenCoins);
            }

            // 모든 코인 애니메이션 완료 대기
            yield return new WaitForSeconds(flyDuration);

            // 코인 오브젝트들 제거
            ClearCoinObjects();
            isAnimatingCoins = false;
        }
        IEnumerator StartNumberAnimation(float duration)
        {
            yield return new WaitForSeconds(duration);
            StartNumberAnimation();
        }

        private IEnumerator AnimateSingleCoin(GameObject coin, Vector2 targetPos, float duration)
        {
            RectTransform rectTransform = coin.GetComponent<RectTransform>();
            Vector2 startPos = rectTransform.anchoredPosition;

            // 곡선 경로를 위한 제어점 생성
            Vector2 controlPoint = GenerateControlPoint(startPos, targetPos);

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // 이징 함수 적용 (부드러운 감속)
                float easeOut = 1f - Mathf.Pow(1f - t, 3f);

                // 베지어 곡선을 사용한 위치 업데이트
                Vector2 currentPos = BezierCurve(startPos, controlPoint, targetPos, easeOut);
                rectTransform.anchoredPosition = currentPos;

                // 크기 변화 (시작할 때 작게, 도착할 때 더 작게)
                float scale = Mathf.Lerp(1f, 0.3f, easeOut);
                rectTransform.localScale = new Vector3(scale, scale, 1f);

                yield return null;
            }

            // 정확한 목표 위치로 설정
            rectTransform.anchoredPosition = targetPos;
            rectTransform.localScale = new Vector3(0.3f, 0.3f, 1f);
        }

        private void StartNumberAnimation()
        {
            _holdNumber = false;
            // 숫자 애니메이션 시작
            Number = new BigNum(targetNumber);
            _previousNumber = originalNumber;
            _timeChecker = 0;
        }

        private IEnumerator SpreadCoins(float duration)
        {
            int coinCount = coinObjects.Count;
            float spreadRadius = CurrencySpreadRadius;

            // 각 코인의 최종 퍼진 위치 계산
            List<Vector2> spreadPositions = new List<Vector2>();
            Vector2 centerPos = coinObjects[0].GetComponent<RectTransform>().anchoredPosition;

            for (int i = 0; i < coinCount; i++)
            {
                float angle = (360f / coinCount) * i;
                float randomRadius = Random.Range(spreadRadius * 0.5f, spreadRadius);
                Vector2 offset = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad) * randomRadius,
                    Mathf.Sin(angle * Mathf.Deg2Rad) * randomRadius
                );
                spreadPositions.Add(centerPos + offset);
            }

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // 이징 함수 (부드러운 가속)
                float easeIn = t * t;

                for (int i = 0; i < coinObjects.Count; i++)
                {
                    RectTransform rectTransform = coinObjects[i].GetComponent<RectTransform>();
                    Vector2 startPos = centerPos;
                    Vector2 endPos = spreadPositions[i];

                    // 위치 업데이트
                    rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, easeIn);

                    // 크기 업데이트 (0에서 1로)
                    float scale = Mathf.Lerp(0f, 1f, easeIn);
                    rectTransform.localScale = new Vector3(scale, scale, 1f);
                }

                yield return null;
            }

            // 정확한 최종 위치로 설정
            for (int i = 0; i < coinObjects.Count; i++)
            {
                RectTransform rectTransform = coinObjects[i].GetComponent<RectTransform>();
                rectTransform.anchoredPosition = spreadPositions[i];
                rectTransform.localScale = Vector3.one;
            }
        }

        private Vector2 GenerateControlPoint(Vector2 startPos, Vector2 endPos)
        {
            // 시작점과 끝점 사이의 거리 계산
            float distance = Vector2.Distance(startPos, endPos);

            // 중간점 계산
            Vector2 midPoint = (startPos + endPos) * 0.5f;

            // 랜덤한 방향으로 제어점 생성
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float controlDistance = distance * Random.Range(0.3f, 0.8f); // 거리의 30%~80%

            Vector2 controlOffset = new Vector2(
                Mathf.Cos(randomAngle) * controlDistance,
                Mathf.Sin(randomAngle) * controlDistance
            );

            return midPoint + controlOffset;
        }

        private Vector2 BezierCurve(Vector2 start, Vector2 control, Vector2 end, float t)
        {
            // 2차 베지어 곡선 공식: B(t) = (1-t)²P₀ + 2(1-t)tP₁ + t²P₂
            float oneMinusT = 1f - t;
            return oneMinusT * oneMinusT * start + 2f * oneMinusT * t * control + t * t * end;
        }

        private void ClearCoinObjects()
        {
            foreach (GameObject coin in coinObjects)
            {
                if (coin != null)
                {
                    DestroyImmediate(coin);
                }
            }
            coinObjects.Clear();
        }
    }
}
