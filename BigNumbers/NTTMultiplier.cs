using System;
using System.Collections.Generic;
using System.Linq;

public static class NTTMultiplier
{
    private const int Mod = 998244353;
    private const int Root = 3;
    private const int BlockSize = 4; // по 4 цифры в блоке
    private const int Base = 10000;  // 10^4

    // -------------------- Вспомогательные методы --------------------

    private static long ModPow(long a, long n)
    {
        long r = 1;
        while (n > 0)
        {
            if ((n & 1) != 0)
                r = (r * a) % Mod;
            a = (a * a) % Mod;
            n >>= 1;
        }
        return r;
    }

    private static void NTT(long[] a, bool invert)
    {
        int n = a.Length;
        for (int i = 1, j = 0; i < n; ++i)
        {
            int bit = n >> 1;
            for (; (j & bit) != 0; bit >>= 1)
                j ^= bit;
            j ^= bit;
            if (i < j)
                (a[i], a[j]) = (a[j], a[i]);
        }

        for (int len = 2; len <= n; len <<= 1)
        {
            long wlen = ModPow(Root, (Mod - 1) / len);
            if (invert)
                wlen = ModPow(wlen, Mod - 2);

            for (int i = 0; i < n; i += len)
            {
                long w = 1;
                for (int j = 0; j < len / 2; ++j)
                {
                    long u = a[i + j];
                    long v = (a[i + j + len / 2] * w) % Mod;
                    a[i + j] = (u + v) % Mod;
                    a[i + j + len / 2] = (u - v + Mod) % Mod;
                    w = (w * wlen) % Mod;
                }
            }
        }

        if (invert)
        {
            long nInv = ModPow(n, Mod - 2);
            for (int i = 0; i < n; ++i)
                a[i] = (a[i] * nInv) % Mod;
        }
    }

    // -------------------- Конвертация --------------------

    private static List<int> ToBlocks(List<byte> bytes)
    {
        var blocks = new List<int>();
        int value = 0;
        int power = 1;
        int count = 0;

        foreach (var digit in Enumerable.Reverse(bytes))
        {
            value += digit * power;
            power *= 10;
            count++;
            if (count == BlockSize)
            {
                blocks.Add(value);
                value = 0;
                power = 1;
                count = 0;
            }
        }

        if (count > 0)
            blocks.Add(value);

        return blocks;
    }

    private static List<byte> FromBlocks(List<long> blocks)
    {
        // blocks: младший блок (LSB) первым
        int hi = blocks.Count - 1;

        // Найдём старший ненулевой блок
        while (hi > 0 && blocks[hi] == 0) hi--;

        var sb = new System.Text.StringBuilder();

        // Старший блок: просто как число (без ведущих нулей)
        sb.Append(blocks[hi].ToString());

        // Остальные блоки: ровно BlockSize цифр с ведущими нулями
        for (int i = hi - 1; i >= 0; i--)
        {
            sb.Append(blocks[i].ToString($"D{BlockSize}"));
        }

        // Превращаем в List<byte>
        var digits = sb.ToString().Select(c => (byte)(c - '0')).ToList();
        return digits;
    }

    // -------------------- Основное умножение --------------------

    public static List<byte> Multiply(List<byte> a, List<byte> b)
    {
        var A = ToBlocks(a);
        var B = ToBlocks(b);

        int n = 1;
        while (n < A.Count + B.Count)
            n <<= 1;

        long[] fa = new long[n];
        long[] fb = new long[n];

        for (int i = 0; i < A.Count; i++) fa[i] = A[i];
        for (int i = 0; i < B.Count; i++) fb[i] = B[i];

        NTT(fa, false);
        NTT(fb, false);

        for (int i = 0; i < n; i++)
            fa[i] = (fa[i] * fb[i]) % Mod;

        NTT(fa, true);

        var res = new List<long>(n);
        long carry = 0;

        for (int i = 0; i < n; i++)
        {
            long val = fa[i] + carry;
            res.Add(val % Base);
            carry = val / Base;
        }

        while (carry > 0)
        {
            res.Add(carry % Base);
            carry /= Base;
        }

        while (res.Count > 1 && res[^1] == 0)
            res.RemoveAt(res.Count - 1);

        return FromBlocks(res);
    }
}