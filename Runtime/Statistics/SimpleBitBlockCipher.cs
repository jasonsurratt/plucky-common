using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Assertions;

namespace Plucky.Common
{
    public sealed class SimpleBitBlockCipher
    {
        public int bits = 64;
        // 100 seems to work pretty well, the professional ciphers can do it in 15 or so. I haven't
        // done any randomness testing, it just "looks" pretty good.
        public int rounds = 100;
        public ulong seed;

        public ulong Decrypt(ulong ciphertext)
        {
            ulong ones = Ones(bits);
            ulong result = ciphertext;

            ulong tmpSeed = Hash(seed);

            for (int i = rounds - 1; i >= 0; i--)
            {
                int shift1 = (int)(ShiftLeft(tmpSeed, (1 * i) % 64, 64) & 0xFFFFFFF) % 64;
                int shift2 = (int)(ShiftLeft(tmpSeed, shift1 % 64, 64) & 0xFFFFFFF) % bits;
                int shift3 = (int)(ShiftRight(tmpSeed, (1 * i) % 64, 64) & 0xFFFFFFF) % 64;
                ulong smallSeed = ShiftLeft(tmpSeed, shift1, 64) & ones;

                ulong r = (ShiftLeft(tmpSeed, shift3, 64) & ones) ^ result;
                r = ShiftLeft(r, shift2, bits);
                r = Sub(r, smallSeed, bits);
                result = r;
            }

            return result;
        }

        public ulong Encrypt(ulong payload)
        {
            ulong ones = Ones(bits);
            ulong result = payload;

            ulong tmpSeed = Hash(seed);

            for (int i = 0; i < rounds; i++)
            {
                int shift1 = (int)(ShiftLeft(tmpSeed, (1 * i) % 64, 64) & 0xFFFFFFF) % 64;
                int shift2 = (int)(ShiftLeft(tmpSeed, shift1 % 64, 64) & 0xFFFFFFF) % bits;
                int shift3 = (int)(ShiftRight(tmpSeed, (1 * i) % 64, 64) & 0xFFFFFFF) % 64;
                ulong smallSeed = ShiftLeft(tmpSeed, shift1, 64) & ones;

                ulong r = Add(result, smallSeed, bits);
                r = ShiftRight(r, shift2, bits);
                result = r ^ (ShiftLeft(tmpSeed, shift3, 64) & ones);
            }

            return result;
        }

        public static ulong Add(ulong a, ulong b, int bits)
        {
            ulong sum = a + b;
            ulong ones = Ones(bits);

#if DEBUG
            Assert.IsTrue(bits <= 64);
            Assert.IsTrue(a <= ones, "a is greater than the max value allowed by the # of bits.");
            Assert.IsTrue(b <= ones, "b is greater than the max value allowed by the # of bits.");
#endif

            ulong result = sum & ones;
            //Debug.Log($"{sum:X} {result:X}");

            return result;
        }

        public ulong Hash(ulong v)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            return (ulong)Math.Abs(BitConverter.ToInt64(sha1.ComputeHash(BitConverter.GetBytes(v)), 0));
        }

        public static ulong BitRange(ulong value, int lowerBit, int upperBit)
        {
            ulong ones = Ones(upperBit - lowerBit);

            return (value >> lowerBit) & ones;
        }

        public static ulong ShiftLeft(ulong a, int shift, int bits)
        {
#if DEBUG
            Assert.IsTrue(bits <= 64);
            Assert.IsTrue(shift <= bits && shift >= 0);
#endif
            ulong ones = Ones(bits);
            ulong result = ((a << shift) | (a >> (bits - shift))) & ones;

            ulong x = a << shift;
            ulong y = a >> (bits - shift);

            //Debug.Log($"{x:X} {y:X} {x | y:X} {Ones(bits)} {result:X}");

            return result;
        }

        public static ulong ShiftRight(ulong a, int shift, int bits)
        {
#if DEBUG
            Assert.IsTrue(bits <= 64);
            Assert.IsTrue(shift <= bits && shift >= 0);
#endif

            ulong result = ((a >> shift) | (a << (bits - shift))) & Ones(bits);

            ulong x = a >> shift;
            ulong y = a << (bits - shift);

            //Debug.Log($"{x:X} {y:X} {x | y:X} {Ones(bits)} {result:X}");

            return result;
        }

        public static ulong Sub(ulong a, ulong b, int bits)
        {
            ulong ones = Ones(bits);

#if DEBUG
            Assert.IsTrue(bits <= 64);
            Assert.IsTrue(a <= ones, "a is greater than the max value allowed by the # of bits.");
            Assert.IsTrue(b <= ones, "b is greater than the max value allowed by the # of bits.");
#endif

            ulong sum = a - b;
            ulong result = sum & ones;
            //Debug.Log($"{sum:X} {result:X}");

            return result;
        }

        public static ulong Ones(int numberOfOnes)
        {
#if DEBUG
            Assert.IsTrue(numberOfOnes <= 64);
#endif
            if (numberOfOnes == 64) return 0xFFFFFFFFFFFFFFFF;
            return ((1UL << numberOfOnes) - 1);
        }
    }
}
