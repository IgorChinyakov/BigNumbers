using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BigNumbersLib
{
    public class BigNumber
    {
        private List<byte> _bytes = [];
        private bool _isNegative = false;

        public bool IsNegative => _isNegative;

        public IReadOnlyList<byte> Bytes => _bytes;

        private BigNumber(List<byte> bytes, bool isNegative)
        {
            _bytes = bytes;
            _isNegative = isNegative;
        }

        public BigNumber(string number)
        {
            if (number.StartsWith('-'))
            {
                _isNegative = true;
            }
            
            for (int i = IsNegative ? 1 : 0; i < number.Length; i++)
            {
                _bytes.Add((byte)(number[i] - '0'));
            }
        }

        public static bool operator <(BigNumber a, BigNumber b) => Compare(a, b) < 0;

        public static bool operator >(BigNumber a, BigNumber b) => Compare(a, b) > 0;

        public static bool operator <=(BigNumber a, BigNumber b) => Compare(a, b) <= 0;

        public static bool operator >=(BigNumber a, BigNumber b) => Compare(a, b) >= 0;

        public static bool operator ==(BigNumber a, BigNumber b) => Compare(a, b) == 0;

        public static bool operator !=(BigNumber a, BigNumber b) => Compare(a, b) != 0;

        public static BigNumber operator ^(BigNumber number, int degree)
        {
            if (degree < 0)
                throw new ArgumentException("Степень должна быть неотрицательной.");

            if (degree == 0)
                return new BigNumber("1");

            if (degree == 1)
                return new BigNumber(number._bytes, number.IsNegative);

            var result = new BigNumber("1");
            var baseValue = new BigNumber(number._bytes, number.IsNegative);

            int exp = degree;

            while (exp > 0)
            {
                // Если степень нечётная — умножаем результат на текущее основание
                if ((exp & 1) == 1)
                    result = result * baseValue;

                // Возводим основание в квадрат
                baseValue = baseValue * baseValue;

                // Делим степень на 2
                exp >>= 1;
            }

            return result;
        }

        public static BigNumber operator +(BigNumber first, BigNumber second)
        {
            if (first._isNegative == second._isNegative)
            {
                var bytes = SumModules(first._bytes, second._bytes);
                return new BigNumber(bytes, second._isNegative);
            }
            else
            {
                int cmp = CompareModules(first._bytes, second._bytes);
                if (cmp >= 0)
                {
                    var resultBytes = SubtractModules(first._bytes, second._bytes);
                    return new BigNumber(resultBytes, first._isNegative);
                }
                else
                {
                    var resultBytes = SubtractModules(second._bytes, first._bytes);
                    return new BigNumber(resultBytes, second._isNegative);
                }
            }
        }

        public static BigNumber operator -(BigNumber first, BigNumber second)
        {
            var negativeSecond = new BigNumber(second._bytes, true);
            return first + negativeSecond;
        }

        public static BigNumber operator *(BigNumber first, BigNumber second)
        {
            //bool resultNegative = first._isNegative != second._isNegative;

            //int n = first._bytes.Count;
            //int m = second._bytes.Count;

            //// Максимальная длина результата — n + m
            //var result = new int[n + m];

            //// Умножаем "в столбик"
            //for (int i = n - 1; i >= 0; i--)
            //{
            //    for (int j = m - 1; j >= 0; j--)
            //    {
            //        int product = first._bytes[i] * second._bytes[j];
            //        int posLow = i + j + 1;
            //        int posHigh = i + j;

            //        product += result[posLow]; // прибавляем к уже накопленной цифре
            //        result[posLow] = product % 10;
            //        result[posHigh] += product / 10;
            //    }
            //}

            //// Преобразуем к List<byte>, убрав ведущие нули
            //int startIndex = 0;
            //while (startIndex < result.Length - 1 && result[startIndex] == 0)
            //    startIndex++;

            //var bytes = result
            //    .Skip(startIndex)
            //    .Select(x => (byte)x)
            //    .ToList();

            //// Если всё нули — делаем положительным
            //bool isZero = bytes.Count == 1 && bytes[0] == 0;
            //return new BigNumber(bytes, isZero ? false : resultNegative);

            var result = new BigNumber("0");
            bool negative = first._isNegative != second._isNegative;

            var resBytes = NTTMultiplier.Multiply(first._bytes, second._bytes);
            result._bytes = resBytes;
            result._isNegative = negative && !(resBytes.Count == 1 && resBytes[0] == 0);

            return result;
        }

        public static BigNumber operator /(BigNumber dividend, BigNumber divisor)
        {
            if (divisor == new BigNumber("0"))
                throw new DivideByZeroException();

            bool resultNegative = dividend._isNegative != divisor._isNegative;

            var quotient = new List<byte>();
            var remainder = new List<byte>();

            foreach (var digit in dividend._bytes)
            {
                remainder.Add(digit);

                // Убираем ведущие нули в remainder
                while (remainder.Count > 1 && remainder[0] == 0)
                    remainder.RemoveAt(0);

                byte count = 0;
                while (CompareModules(remainder, divisor._bytes) >= 0)
                {
                    remainder = SubtractModules(remainder, divisor._bytes);
                    count++;
                }

                quotient.Add(count);
            }

            // Убираем ведущие нули в частном
            while (quotient.Count > 1 && quotient[0] == 0)
                quotient.RemoveAt(0);

            var result = new BigNumber(quotient, resultNegative);

            // Если результат = 0, делаем знак положительным
            if (result._bytes.Count == 1 && result._bytes[0] == 0)
                result._isNegative = false;

            return result;
        }

        public void AddByte(byte @byte)
            => _bytes.Add(@byte);

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            int resultLength = _bytes.Count;

            if(IsNegative) stringBuilder.Append('-');
            for (int i = 0; i < resultLength; i++)
            {
                stringBuilder.Append(_bytes[i]);
            }

            return stringBuilder.ToString();
        }

        private static List<byte> SumModules(List<byte> first, List<byte> second)
        {
            var firstBytes = new List<byte>(first);
            var secondBytes = new List<byte>(second);

            while (firstBytes.Count < secondBytes.Count) firstBytes.Insert(0, 0);
            while (secondBytes.Count < firstBytes.Count) secondBytes.Insert(0, 0);

            var resultBytes = new Stack<byte>();
            var sumAndCarry = ((byte)0, (byte)0);
            for (int i = firstBytes.Count - 1; i >= 0; i--)
            {
                sumAndCarry = SumBytes(firstBytes[i], secondBytes[i], sumAndCarry.Item2);
                resultBytes.Push(sumAndCarry.Item1);
            }

            if (sumAndCarry.Item2 > 0)
            {
                resultBytes.Push(sumAndCarry.Item2);
            }

            return [.. resultBytes];
        }

        private static List<byte> SubtractModules(List<byte> first, List<byte> second)
        {
            var firstBytes = new List<byte>(first);
            var secondBytes = new List<byte>(second);

            while (secondBytes.Count < firstBytes.Count) secondBytes.Insert(0, 0);

            var result = new Stack<byte>();
            byte borrow = 0;

            for (int i = firstBytes.Count - 1; i >= 0; i--)
            {
                int diff = firstBytes[i] - secondBytes[i] - borrow;
                if (diff < 0)
                {
                    diff += 10;
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }
                result.Push((byte)diff);
            }

            while (result.Count > 1 && result.Peek() == 0)
                result.Pop();

            return [.. result];
        }


        private static (byte, byte) SumBytes(byte first, byte second, byte carry)
        {
            var sum = first + second + carry;
            return ((byte)(sum % 10), (byte)(sum / 10));
        }

        private static int Compare(BigNumber first, BigNumber second)
        {
            // Сравнение знаков
            if (first.IsNegative && !second.IsNegative) return -1;
            if (!first.IsNegative && second.IsNegative) return 1;

            // Определяем коэффициент для отрицательных чисел
            int signFactor = first.IsNegative ? -1 : 1;

            // Сравнение длины
            if (first._bytes.Count != second._bytes.Count)
                return first._bytes.Count < second._bytes.Count ? -1 * signFactor : 1 * signFactor;

            // Побайтовое сравнение
            for (int i = 0; i < first._bytes.Count; i++)
            {
                if (first._bytes[i] != second._bytes[i])
                    return first._bytes[i] < second._bytes[i] ? -1 * signFactor : 1 * signFactor;
            }

            // Если все байты равны
            return 0;
        }

        private static int CompareModules(List<byte> first, List<byte> second)
        {
            if (first.Count != second.Count)
                return first.Count < second.Count ? -1 : 1;

            for (int i = 0; i < first.Count; i++)
            {
                if (first[i] != second[i])
                    return first[i] < second[i] ? -1 : 1;
            }
            return 0; 
        }
    }
}
