using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
//using Microsoft.Unity.VisualStudio.Editor;
//using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace StudioNAP
{
    [Serializable]
    public class BigNum
    {
        //private char spliter = '_';
        //int unitIndex = 0;
        public List<double> numbers = new List<double>();
        public double _remainder = 0;
        public BigNum()
        {
            resetNum();
        }
        public BigNum(int num) : this()
        {
            AddNum(num, 0);
        }
        public BigNum(BigInteger num) : this()
        {
            AddNum(num, 0);
        }
        public BigNum(long num) : this()
        {
            AddNum(num, 0);
        }
        public BigNum(double num) : this()
        {
            // Debug.Log($"bignum double num: {num}");
            AddNum(num);
        }
        public BigNum(float num) : this()
        {
            AddNum((double)num, 0);
        }

        public BigNum(string str) : this()
        {
            if (string.IsNullOrEmpty(str))
            {
                resetNum();
            }
            else if (str.Contains("E+"))
            {
                BigNum num = Parse(str);
                numbers = new List<double>(num.numbers);
            }
            else if (str.StartsWith("s")) // short expression
            {
                // Debug.Log("short bignum str: " + str);
                string[] strs = str.Split('_');
                numbers.Clear();
                int previousCount = int.Parse(strs[0].Substring(1));
                for (int i = 0; i < previousCount; i++)
                {
                    numbers.Add(0);
                }
                numbers.Add(int.Parse(strs[strs.Length - 1]));
                numbers.Add(int.Parse(strs[strs.Length - 2]));
                //Debug.Log("hp short from: " + GetExpression());
            }
            else if (str.Contains(","))
            {
                SetNumFromFullNumberByCommaSplitString(str);
            }
            else if (str.All(char.IsDigit))
            {
                int length = str.Length;
                int remainder = length % 3;
                if (remainder > 0)
                {
                    string part = str.Substring(0, remainder);
                    int number = int.Parse(part);
                    AddNum(number, length / 3);
                }
                for (int i = remainder; i < length; i += 3)
                {
                    string part = str.Substring(i, 3);
                    int number = int.Parse(part);
                    AddNum(number, (length - i - 3) / 3);
                }
            }
            else
            {
                SetNumFromFullNumberByCommaSplitString(str);
            }
        }
        public BigNum(BigNum init) : this()
        {
            AddNum(init);
        }
        public void Initialize()
        {
            if (numbers == null) numbers = new List<double>();
        }
        public bool IsSame(BigNum other)
        {
            if (this.IsZeroOrUnder() && other.IsZeroOrUnder())
                return true;

            if (this.numbers == null || other.numbers == null)
                return false;

            int maxCount = Mathf.Max(this.numbers.Count, other.numbers.Count);

            for (int i = 0; i < maxCount; i++)
            {
                double a = (i < this.numbers.Count) ? this.numbers[i] : 0;
                double b = (i < other.numbers.Count) ? other.numbers[i] : 0;

                if (Math.Abs(a - b) > 0.0001f) // float 오차 방지
                    return false;
            }
            return true;
        }


        public List<double> GetNumbers()
        {
            if (numbers == null) numbers = new List<double>();
            return numbers;
        }
        public string GetShortExpression()
        {
            if (numbers.Count < 2)
            {
                return GetNumForSave();
            }
            else
            {
                return $"s{numbers.Count - 2}_{numbers[numbers.Count - 1]}_{numbers[numbers.Count - 2]}";
            }
        }
        public static BigNum GetNumFromString(string str)
        {
            BigNum num = new BigNum();
            num.numbers = SplitNumberString(str);
            return num;
        }
        public static List<double> SplitNumberString(string str)
        {
            // Reverse the input string to make it easier to split into groups of up to three digits
            char[] charArray = str.ToCharArray();
            Array.Reverse(charArray);
            string reversedString = new string(charArray);

            // Split the reversed string into groups of up to three digits
            List<string> groups = new List<string>();
            for (int i = 0; i < reversedString.Length; i += 3)
            {
                int length = Mathf.Min(3, reversedString.Length - i);
                groups.Add(reversedString.Substring(i, length));
            }

            // Convert each group back to an integer and store in the result list
            List<double> result = new List<double>();
            for (int i = 0; i < groups.Count; i++)
            {
                charArray = groups[i].ToCharArray();
                Array.Reverse(charArray);
                result.Add(int.Parse(new string(charArray)));
            }

            // Reverse the result list to restore the original order
            //result.Reverse();
            return result;
        }

        public void resetNum()
        {
            numbers.Clear();
            numbers.Add(0);
        }
        public void SetNumFromFullNumberByCommaSplitString(string str)
        {
            numbers.Clear();
            //Debug.Log("full str: " + str);
            //ValueVector rows = GameManager::getInstance().split(str, splitChar);
            string[] rows = str.Split('_');
            string num;
            //Debug.Log("num str: " + str);
            for (int i = 0; i < rows.Length; i++)
            {
                num = rows[i];
                //        Value(num).asString();
                if (string.IsNullOrEmpty(num)) continue;
                //Debug.Log(string.Format("num: {0}", num));

                int intNum;
                if (int.TryParse(num, out intNum)) numbers.Add(intNum);
                else
                {
                    Debug.Log("BigNum str format exception: " + num);
                    resetNum();
                }
            }
        }
        public bool IsZeroOrUnder()
        {
            for (int i = numbers.Count - 1; i >= 0; i--)
            {
                if (numbers[i] > 0) return false;
                if (numbers[i] < 0) return true;
            }
            //return numbers.Count >= 1 && numbers[0] <= 0;
            return true;
        }
        public BigInteger GetBigInteger()
        {
            BigInteger num = 0;
            for (int i = 0; i < numbers.Count; i++)
            {
                num += (int)numbers[i] * BigInteger.Pow(1000, i);
                // Debug.Log("add to num: " + numbers[i] + " * " + Mathf.Pow(1000, i));
            }
            // Debug.Log("num: " + num);
            // Debug.Log("numForSave: " + GetNumForSave());
            return num;
        }
        public string GetNumForSave()
        {
            StringBuilder str = new StringBuilder();
            string num;
            for (int i = 0; i < (int)numbers.Count; i++)
            {
                num = numbers[i].ToString();
                str.Append(num);
                str.Append('_');
                //Debug.Log("saveuser spliter: " + '_');
            }
            if (numbers.Count > 0) str.Length--;
            return str.ToString();
        }
        public void AddNum(double amount)
        {
            if (amount < 1000000)
            {
                AddNum((int)amount);
                return;
            }
            //Debug.Log("double amount; " + amount);
            for (int i = 0; amount > 0; i++)
            {
                //Debug.Log($"i: {i}/{amount}");
                int smallNum = (int)(amount % 1000);
                amount /= 1000;
                //Debug.Log($"amnount: {smallNum}/{amount}");
                AddNum(smallNum, i);

                if (amount < 1) break;

                if (i > 900)
                {
                    Debug.Log("something's wrong!!!!");
                    break;
                }
            }
            //AddNum(amount, 0);
        }
        public void AddNum(long amount)
        {
            AddNum(amount, 0);
        }
        public void AddNum(BigInteger amount)
        {
            AddNum(amount, 0);
        }
        public void AddNum(int amount)
        {
            AddNum(amount, 0);
        }
        public void AddNum(BigInteger amount, int unit)
        {
            //Debug.Log("what: " + amount);
            for (int i = 0; amount > 0; i++)
            {
                for (int j = (int)numbers.Count; j <= i; j++)
                { // create if not exist
                    numbers.Add(0);
                }
                numbers[i] += (int)(amount % 1000);
                amount /= 1000;
            }
            Arrange();
        }
        public void AddNum(long amount, int unit)
        {
            AddNum((double)amount, unit);
        }
        public void AddNum(double amount, int unit)
        {
            for (int i = numbers.Count; i < unit + 1; i++)
            { // create if not exist
                numbers.Add(0);
            }
            numbers[unit] += amount;
            Arrange();
        }
        public void AddNum(int amount, int unit)
        {
            //Debug.Log("what int: " + amount);
            for (int i = (int)numbers.Count; i < unit + 1; i++)
            { // create if not exist
                numbers.Add(0);
            }
            numbers[unit] += amount;
            Arrange();
        }
        public void AddNum(BigNum num)
        {
            //Debug.Log(string.Format("num: {0}", num));
            for (int i = (int)GetNumbers().Count; i < num.GetNumbers().Count; i++)
            { // create if not exist
                GetNumbers().Add(0);
            }

            for (int i = 0; i < num.GetNumbers().Count; i++)
            {
                numbers[i] += num.GetNumbers()[i];
            }
            Arrange();
        }
        public void SubtractNum(int amount, int unit)
        {
            AddNum(-amount, unit);
        }
        public void SubtractNum(BigNum num)
        {
            for (int i = (int)numbers.Count; i < num.GetNumbers().Count; i++)
            { // create if now exist
                numbers.Add(0);
            }

            for (int i = 0; i < num.GetNumbers().Count; i++)
            {
                numbers[i] -= num.GetNumbers()[i];
            }
            if (numbers[numbers.Count - 1] < 0)
            {
                //Debug.Log("bigNum minus!");
                resetNum();
            }
            else
            {
                Arrange();
            }
        }
        public void Arrange()
        {
            bool positiveExist = false;
            bool negativeExist = false;
            bool biggestNumIsPositive = false;
            double num = 0;
            for (int i = 0; i < numbers.Count; i++)
            {
                num = numbers[i];
                if (num != 0)
                {
                    biggestNumIsPositive = num > 0;
                }
                if (num > 0)
                {
                    positiveExist = true;
                }
                if (num < 0)
                {
                    negativeExist = true;
                }
                if (num >= 1000 || num < -1000)
                {
                    numbers[i] = num % 1000;
                    AddNum((num / 1000), i + 1);
                }
            }

            if (biggestNumIsPositive && negativeExist)
            {
                for (int i = 0; i < numbers.Count - 1; i++)
                {
                    num = numbers[i];
                    if (num < 0)
                    {
                        numbers[i + 1]--;
                        numbers[i] += 1000;
                    }
                }
            }
            else if (!biggestNumIsPositive && positiveExist)
            {
                for (int i = 0; i < numbers.Count - 1; i++)
                {
                    num = numbers[i];
                    if (num > 0)
                    {
                        numbers[i + 1]++;
                        numbers[i] -= 1000;
                    }
                }
            }
            RemoveUnusedHigherZero();
            for (int i = 0; i < numbers.Count; i++)
            {
                if (i == 0)
                {
                    int numInt = (int)numbers[i];
                    if (numInt < numbers[i])
                    {
                        _remainder = numbers[i] - numInt;
                    }
                }
                numbers[i] = (int)numbers[i];
            }
        }
        public void RemoveUnusedHigherZero()
        {
            while (numbers[numbers.Count - 1] == 0 && numbers.Count > 1)
            {
                numbers.RemoveAt(numbers.Count - 1);
            }
        }
        public int IsBiggerThanThis(int num)
        {
            return IsBiggerThanThis(new BigNum(num));
        }
        /// <summary>
        /// 1 bigger, -1 smaller, 0 equal
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public int IsBiggerThanThis(BigNum num)
        {
            if (num == null)
            {
                num = 0;
            }
            RemoveUnusedHigherZero();
            num.RemoveUnusedHigherZero();
            if (numbers[numbers.Count - 1] * num.GetNumbers()[num.GetNumbers().Count - 1] == 0) // somthing is 0
            {
                if (numbers[numbers.Count - 1] > num.GetNumbers()[num.GetNumbers().Count - 1])
                {
                    return 1;
                }
                else if (numbers[numbers.Count - 1] < num.GetNumbers()[num.GetNumbers().Count - 1])
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
            else if (numbers[numbers.Count - 1] * num.GetNumbers()[num.GetNumbers().Count - 1] < 0) // nagative and positive
            {
                if (numbers[numbers.Count - 1] > 0)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else if (numbers[numbers.Count - 1] < 0 && num.GetNumbers()[num.GetNumbers().Count - 1] < 0) // both nagative 
            {
                if (numbers.Count != num.GetNumbers().Count)
                {
                    if (numbers.Count > num.GetNumbers().Count)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            else // both positive
            {

                if (numbers.Count != num.GetNumbers().Count)
                {
                    if (numbers.Count > num.GetNumbers().Count)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            int index = (int)numbers.Count - 1;
            while (index >= 0)
            {
                if (numbers[index] != num.GetNumbers()[index])
                {
                    if (numbers[index] > num.GetNumbers()[index])
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                index--;
            }
            return 0;
        }
        public void Multiply(int rate)
        {
            Multiply((double)rate);
        }
        public void Multiply(float rate)
        {
            Multiply((double)rate);
        }
        public void Multiply(double rate)
        {
            double num;
            double rated;
            //int integeredNum;
            double dataToAddLowerLevel = 0;

            for (int i = numbers.Count - 1; i >= 0; i--)
            {
                num = numbers[i];
                if (i == 0) num += _remainder;
                rated = num * rate;
                //integeredNum = (int)(rated + 0.5f); // round off

                numbers[i] = (long)rated;
                numbers[i] += (long)dataToAddLowerLevel;
                dataToAddLowerLevel = ((rated * 1000)) % 1000;
                // if (i > 0 && dataToAddLowerLevel > 0)
                // {
                //     // dataToAddLowerLevel += dataToAddLowerLevel;
                //     // dataToAddLowerLevel = (int)(rated - integeredNum) * 1000;
                // }
                // else
                // {
                //     dataToAddLowerLevel = 0;
                // }
            }
            Arrange();
        }
        public void Multiply(BigNum rate)
        {
            if (rate.numbers.Count < 2)
            {
                Multiply((float)rate.ToNum());
            }
            else
            {
                for (int i = 2; i < rate.numbers.Count; i++)
                {
                    numbers.Insert(0, 0);
                }
                double toMultiply = rate.numbers[rate.numbers.Count - 1] * 1000 + rate.numbers[rate.numbers.Count - 2];
                Multiply(toMultiply);
            }
            //BigNum newNum = new BigNum(0);
            //for (int i = rate.GetNumbers().Count - 1; i >= 0; i++)
            //{
            //    BigNum curPosNum = new BigNum(this);
            //    for (int j = 0; j < i; j++)
            //    {
            //        curPosNum.GetNumbers().Insert(0, 0);
            //    }
            //    curPosNum.Multiply(rate.GetNumbers()[i]);
            //    newNum.AddNum(curPosNum);
            //}

            //SetNum(newNum);
        }
        public string GetUnitStr(int unit)
        {
            if (unit <= 0)
            {
                return string.Empty;
            }
            unit = unit - 1;

            int devider = 26;
            int loop = 1 + unit / devider;
            int index = unit % devider;
            char ch = (char)((int)'A' + index);
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < loop; i++)
            {
                str.Append(ch);
            }
            return str.ToString();
        }
        public string GetFullNumberStringByCommaSplit()
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < numbers.Count; i++)
            {
                str.Append(numbers[i].ToString());
                str.Append('_');
            }
            string finalStr = str.ToString();
            return finalStr.Substring(0, finalStr.Length - 1);
        }
        public long ToNum()
        {
            long total = 0;
            for (int i = 0; i < numbers.Count; i++)
            {
                total += (long)(numbers[i] * Math.Pow(1000, i));
            }
            return total;
        }
        public double ToDouble()
        {
            double total = 0;
            for (int i = 0; i < numbers.Count; i++)
            {
                total += numbers[i] * Math.Pow(1000, i);
            }
            return total + _remainder;
        }
        public string ToFullNumber()
        {
            string str = string.Empty;
            foreach (var num in numbers)
            {
                str.Insert(0, num.ToString());
            }
            return str;
        }
        public override string ToString()
        {
            if (this == null)
            {
                Debug.Log("this is null");
                return "0";
            }
            //Debug.Log(string.Format("bignum tostring " + numbers.Count));
            return GetExpression();
        }
        public double GetLastNumber()
        {
            return numbers[numbers.Count - 1];
        }
        public string ToStringFiveChars()
        {
            int unitCount = 0;
            int places = numbers.Count;
            //Debug.Log(string.Format("{0}/{1}", unitCount, places));
            //Debug.Log("places: " + places);
            for (int i = 0; i < 100; i++)
            {
                if (places > 2)
                {
                    places--;
                    unitCount++;
                }
                else
                {
                    break;
                }
            }
            if (places == 2)
            {
                if (numbers[numbers.Count - 1] >= 100)
                {
                    places--;
                    unitCount++;
                }
            }

            string str = string.Empty;
            for (int i = 0; i < places; i++)
            {
                if (i == 0)
                {
                    str += numbers[numbers.Count - 1 - i].ToString();
                }
                else
                {
                    str += numbers[numbers.Count - 1 - i].ToString("000");
                }
            }
            str += GetUnitStr(unitCount);
            //if (numbers.Count > 0 &&  numbers[1] == 100)
            //{
            //    Debug.Log(string.Format("{0}/{1}/{2}", unitCount, places, str));
            //}

            return str;

            //if (numbers.Count > 5)
            //{// 1 000 000
            //    int index = numbers.Count-3
            //    str.Append(numbers[numbers.Count - 1]);
            //    int firstDigitPlace = (int)str.Length;
            //    str = str.Append(splitChar);
            //    int digitPlaceLeft = 4 - firstDigitPlace;
            //    digitPlaceLeft = 1;
            //    str = str.Append(GetZeroBaseNum(numbers[numbers.Count - 2], true).Substring(0, digitPlaceLeft));
            //    str = str.Append(GetUnitStr((int)numbers.Count - 1));
            //    return str.ToString();
            //}
            //else
            //{
            //    return numbers[0].ToString();
            //}

        }
        public string GetExpression()
        {
            // Debug.Log("get expressino:" + GetNumForSave());
            StringBuilder str = new StringBuilder();
            if (numbers.Count == 2 && ToNum() < 100000)
            {
                return ToNum().ToString();
            }
            if (numbers.Count > 1)
            {

                str.Append(numbers[numbers.Count - 1]);
                int firstDigitPlace = (int)str.Length;
                str = str.Append('.');
                int digitPlaceLeft = 4 - firstDigitPlace;
                str = str.Append(GetZeroBaseNum(numbers[numbers.Count - 2], true).Substring(0, digitPlaceLeft));
                str = str.Append(GetUnitStr((int)numbers.Count - 1));

                return str.ToString();
            }
            else
            {
                return numbers[0].ToString();
            }
        }
        public string GetZeroBaseNum(double num, bool removeNegative)
        {
            StringBuilder str = new StringBuilder();
            bool isNegative = num < 0;
            if (isNegative)
            {
                num *= -1;
                if (!removeNegative)
                {
                    str.Append("-");
                }
            }
            if (num >= 100)
            {
                ;
            }
            else if (num >= 10)
            {
                str = str.Append("0");
            }
            else
            {
                str = str.Append("00");
            }

            return str.Append(num.ToString()).ToString();
        }

        public void SetNum(BigNum num)
        {
            numbers.Clear();
            for (int i = 0; i < num.GetNumbers().Count; i++)
            {
                numbers.Add(num.GetNumbers()[i]);
            }
        }
        public static BigNum operator +(BigNum a, double b)
        {
            BigNum num = new BigNum(a);
            num.AddNum(b);
            return num;
        }
        public static BigNum operator +(BigNum a, int b)
        {
            BigNum num = new BigNum(a);
            num.AddNum(b);
            return num;
        }
        public static BigNum operator +(BigNum a, BigNum b)
        {
            BigNum num = new BigNum(a);
            num.AddNum(b);
            return num;
        }
        public static BigNum operator -(BigNum a, double b)
        {
            BigNum num = new BigNum(a);
            num.SubtractNum(b);
            return num;
        }
        public static BigNum operator -(BigNum a, BigNum b)
        {
            BigNum num = new BigNum(a);
            num.SubtractNum(b);
            return num;
        }
        //public static BigNum operator *(BigNum a, BigNum b)
        //{
        //    BigNum num = new BigNum(a);
        //    num.Multiply(b);
        //    return num;
        //}
        public static BigNum operator *(BigNum a, float b)
        {
            BigNum num = new BigNum(a);
            num.Multiply(b);
            return num;
        }
        public static BigNum operator *(BigNum a, double b)
        {
            BigNum num = new BigNum(a);
            num.Multiply(b);
            return num;
        }
        public static BigNum operator *(float a, BigNum b)
        {
            BigNum num = new BigNum(b);
            num.Multiply(a);
            return num;
        }
        public static implicit operator BigNum(int value)
        {
            return new BigNum(value);
        }

        public static implicit operator BigNum(long value)
        {
            return new BigNum(value);
        }

        public static implicit operator BigNum(double value)
        {
            return new BigNum(value);
        }

        public static implicit operator string(BigNum value)
        {
            return value.ToString();
        }

        public static double operator /(BigNum a, BigNum b)
        {
            int decimalDiff = a.GetNumbers().Count - b.GetNumbers().Count;
            if (decimalDiff > 1 || decimalDiff < -1) // a > b
            {
                return 0;
            }
            else if (decimalDiff == 1) // a > b
            {
                return (a.GetNumbers()[a.GetNumbers().Count - 1] * 1000f + a.GetNumbers()[a.GetNumbers().Count - 2]) / b.GetNumbers()[b.GetNumbers().Count - 1];
            }
            else if (decimalDiff == -1) // a < b
            {
                return a.GetNumbers()[a.GetNumbers().Count - 1] / (b.GetNumbers()[b.GetNumbers().Count - 1] * 1000f + b.GetNumbers()[b.GetNumbers().Count - 2]);
            }
            else if (decimalDiff == 0) // a == b
            {
                if ((a.GetNumbers()[a.GetNumbers().Count - 1] < 100 || b.GetNumbers()[b.GetNumbers().Count - 1] < 100) &&
                    a.GetNumbers().Count > 1 && b.GetNumbers().Count > 1)
                {
                    double aNumber = a.GetNumbers()[a.GetNumbers().Count - 1] * 1000 + a.GetNumbers()[a.GetNumbers().Count - 2];
                    double bNumber = b.GetNumbers()[b.GetNumbers().Count - 1] * 1000 + b.GetNumbers()[b.GetNumbers().Count - 2];

                    return aNumber / bNumber;
                }
                else
                {
                    return a.GetNumbers()[a.GetNumbers().Count - 1] * 1f / b.GetNumbers()[b.GetNumbers().Count - 1];
                }

            }
            else if (decimalDiff == -1) // a < b
            {
                return a.GetNumbers()[a.GetNumbers().Count - 1] / (b.GetNumbers()[b.GetNumbers().Count - 1] * 1000f);
            }
            else if (decimalDiff < -1) // a < b
            {
                return 0;
            }
            return 0;
        }

        public static BigNum operator /(BigNum a, int b)
        {
            double value = b;
            value = Math.Pow(value, -1);
            BigNum num = new BigNum(a);
            num.Multiply(value);
            return num;
        }
        public static bool operator >(int a, BigNum b)
        {
            BigNum num = new BigNum(a);
            return num.IsBiggerThanThis(b) > 0;
        }
        public static bool operator <(int a, BigNum b)
        {
            BigNum num = new BigNum(a);
            return num.IsBiggerThanThis(b) < 0;
        }
        public static bool operator >(BigNum a, BigNum b)
        {
            if (b == null)
            {
                return a.IsBiggerThanThis(0) > 0;
            }
            else if (a == null)
            {
                return b.IsBiggerThanThis(0) > 0;
            }
            return a.IsBiggerThanThis(b) > 0;
        }
        public static bool operator <(BigNum a, BigNum b)
        {
            return a.IsBiggerThanThis(b) < 0;
        }
        public static bool operator >(BigNum a, int b)
        {
            return a.IsBiggerThanThis(b) > 0;
        }
        public static bool operator <(BigNum a, int b)
        {
            return a.IsBiggerThanThis(b) < 0;
        }

        public static bool operator ==(int a, BigNum b)
        {
            BigNum num = new BigNum(a);
            return num.IsBiggerThanThis(b) == 0;
        }
        public static bool operator !=(int a, BigNum b)
        {
            BigNum num = new BigNum(a);
            return num.IsBiggerThanThis(b) != 0;
        }
        //public static bool operator ==(BigNum a, BigNum b)
        //{
        //    return a.IsBiggerThanThis(b) == 0;
        //}
        //public static bool operator !=(BigNum a, BigNum b)
        //{
        //    return a.IsBiggerThanThis(b) != 0;
        //}
        public static bool operator ==(BigNum a, int b)
        {
            BigNum num = new BigNum(b);
            return num.IsBiggerThanThis(a) == 0;
        }

        public static bool operator !=(BigNum a, int b)
        {
            BigNum num = new BigNum(b);
            return num.IsBiggerThanThis(a) != 0;
        }
        public static bool operator >=(int a, BigNum b)
        {
            BigNum num = new BigNum(a);
            return num.IsBiggerThanThis(b) >= 0;
        }
        public static bool operator <=(int a, BigNum b)
        {
            BigNum num = new BigNum(a);
            return num.IsBiggerThanThis(b) <= 0;
        }
        public static bool operator >=(BigNum a, BigNum b)
        {
            return a.IsBiggerThanThis(b) >= 0;
        }

        public static bool operator <=(BigNum a, BigNum b)
        {
            return a.IsBiggerThanThis(b) <= 0;
        }
        public static bool operator >=(BigNum a, int b)
        {
            return a.IsBiggerThanThis(b) >= 0;
        }
        public static bool operator <=(BigNum a, int b)
        {
            return a.IsBiggerThanThis(b) <= 0;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            BigNum other = (BigNum)obj;
            return this == other;
        }
        public override int GetHashCode()
        {
            return this.GetHashCode();
        }
        public static double ParseDouble(string str)
        {
            double result = 0;
            if (double.TryParse(str, out result))
            {
                return result;
            }
            return 0;
        }
        public static BigNum Parse(string str)
        {
            BigNum num = new BigNum();
            if (string.IsNullOrEmpty(str))
            {
                return num;
            }
            if (str.Contains("E+"))
            {
                int eIndex = str.IndexOf("E+");
                int eCount = int.Parse(str.Substring(eIndex + 2));
                double firstNum = ParseDouble(str.Substring(0, eIndex));
                num.numbers.Clear();
                if (eCount > 3)
                {
                    int rest = eCount % 3;
                    int firstNumInt;
                    if (rest == 0)
                    {
                        rest = 3;
                    }
                    eCount -= rest;
                    firstNumInt = (int)(firstNum * Math.Pow(10, rest));
                    num.AddNum(firstNumInt);
                    // num.numbers.Add(firstNumInt);
                    for (int i = 0; i < eCount; i += 3)
                    {
                        num.Multiply(1000);
                    }

                    // firstNum *= Math.Pow(10, 3);
                    // num.AddNum((long)firstNum);
                    // for (int i = 3; i < eCount; i++)
                    // {
                    //     num.Multiply(10);
                    // }
                }
                else
                {
                    firstNum *= Math.Pow(10, eCount);
                    num.AddNum((long)firstNum);
                }
                // num.Arrange();
                // Debug.Log($"E+ num result2: {num}/ {str}");
            }
            else if (str.EndsWith("0"))
            {
                bool isAllZero = true;
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] != '0')
                    {
                        isAllZero = false;
                        break;
                    }
                }
                if (isAllZero) return 0;

                int zeroStartIndex = 0;
                for (int i = str.Length - 1; i > 0; i--)
                {
                    if (str[i] == '0') zeroStartIndex = i;
                    else break;
                }
                num.AddNum(long.Parse(str.Substring(0, zeroStartIndex)));
                for (int i = zeroStartIndex; i < str.Length; i++)
                {
                    num.Multiply(10);
                }
            }
            else
            {
                num.AddNum(BigInteger.Parse(str));
            }
            return num;
        }
    }
}