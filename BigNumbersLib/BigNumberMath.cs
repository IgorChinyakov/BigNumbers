using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigNumbersLib
{
    public static class BigNumberMath
    {
        // ------------------ Вспомогательные методы ------------------

        private static BigNumber Mod(BigNumber a, BigNumber b)
        {
            // a % b через вычитание сдвигом
            if (b == new BigNumber("0")) throw new DivideByZeroException();
            BigNumber tempA = new BigNumber(a.ToString());
            BigNumber tempB = new BigNumber(b.ToString());

            while (tempA >= tempB)
            {
                // Определяем, на сколько позиций сдвинуть tempB
                int shift = tempA.Bytes.Count - tempB.Bytes.Count;
                BigNumber shiftedB = tempB.ShiftLeft(shift); // умножаем на 10^shift
                if (shiftedB > tempA)
                {
                    shift--;
                    shiftedB = tempB.ShiftLeft(shift);
                }
                tempA -= shiftedB;
            }
            return tempA;
        }

        // Умножение на 10^n (сдвиг влево)
        private static BigNumber ShiftLeft(this BigNumber num, int n)
        {
            if (num.IsZero()) return new BigNumber("0");
            var result = new BigNumber(num.ToString());
            for (int i = 0; i < n; i++)
                result.AddByte(0);
            return result;
        }

        private static bool IsZero(this BigNumber num)
            => num.Bytes.Count == 1 && num.Bytes[0] == 0;

        // ------------------ НОД ------------------

        public static BigNumber GCD(BigNumber a, BigNumber b)
        {
            BigNumber tempA = new BigNumber(a.ToString());
            BigNumber tempB = new BigNumber(b.ToString());

            while (!tempB.IsZero())
            {
                BigNumber r = Mod(tempA, tempB);
                tempA = tempB;
                tempB = r;
            }
            return tempA;
        }

        // ------------------ НОК ------------------

        public static BigNumber LCM(BigNumber a, BigNumber b)
        {
            if (a.IsZero() || b.IsZero()) return new BigNumber("0");
            BigNumber g = GCD(a, b);
            BigNumber divided = a / g; // реализуй деление для BigNumber
            return divided * b;
        }
    }
}
