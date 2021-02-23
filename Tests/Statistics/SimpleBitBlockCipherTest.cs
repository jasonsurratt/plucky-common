using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plucky.Common
{
    public class SimpleBitBlockCipherTest
    {

        [Test]
        public void CipherTest()
        {
            SimpleBitBlockCipher uut = new SimpleBitBlockCipher() { bits = 13, seed = 0xDEADBEEF };

            var inputs = new[]
            {
                new { payload = (ulong)0xF },
                new { payload = (ulong)0xF },
                new { payload = (ulong)0xA },
                new { payload = (ulong)0xFF },
                new { payload = (ulong)0x1234 },
                new { payload = (ulong)0x1555 },
            };

            foreach (var input in inputs)
            {
                ulong e = uut.Encrypt(input.payload);
                ulong d = uut.Decrypt(e);
                Assert.AreEqual(input.payload, d, $"expected {input.payload:X} actual: {d:X}");
            }
        }

        [Test]
        public void CipherRandomTest()
        {
            ulong Rand(ulong max)
            {
                ulong s = (ulong)((long)UnityEngine.Random.Range(-int.MaxValue, int.MaxValue) + int.MaxValue);
                ulong r = (ulong)((long)UnityEngine.Random.Range(-int.MaxValue, int.MaxValue) + int.MaxValue);

                return ((s << 32) + r) % max;
            }

            for (int i = 0; i < 1000; i++)
            {
                int bits = UnityEngine.Random.Range(1, 64);
                ulong ones = SimpleBitBlockCipher.Ones(bits);
                ulong seed = Rand(ones);

                SimpleBitBlockCipher uut = new SimpleBitBlockCipher() { bits = bits, seed = seed };

                ulong payload = Rand(ones);
                ulong e = uut.Encrypt(payload);
                ulong d = uut.Decrypt(e);
                Debug.Log($"ones: {ones:X} bits: {bits} seed: {seed:X} payload: {payload:X} ciphertext: {e:X}");
                Assert.AreEqual(payload, d, $"expected {payload:X} actual: {d:X}");
            }
        }

        [Test]
        public void CipherSequenceTest()
        {
            var inputs = new[]
            {
                new { bits = 5, seed = 0UL },
                new { bits = 10, seed = (ulong)0xDEADBEEF },
            };

            foreach (var input in inputs)
            {
                SimpleBitBlockCipher uut = new SimpleBitBlockCipher() { bits = input.bits, seed = input.seed };

                HashSet<ulong> values = new HashSet<ulong>();
                ulong ones = SimpleBitBlockCipher.Ones(input.bits);
                for (ulong i = 0; i < ones; i++)
                {
                    ulong e = uut.Encrypt(i);
                    ulong d = uut.Decrypt(e);
                    Debug.Log($"i: {i} e: {e} ones: {ones}");
                    Assert.AreEqual(i, d, $"expected {i:X} actual: {d:X}");
                    values.Add(e);
                }
                Assert.AreEqual(values.Count, ones);
            }
        }

        [Test]
        public void AddSubTest()
        {
            void T(ulong a, ulong b, int bits)
            {
                ulong r = SimpleBitBlockCipher.Add(a, b, bits);
                ulong s = SimpleBitBlockCipher.Sub(r, b, bits);

                Assert.AreEqual(s, a);
            }

            T(1000, 50, 10);
            T(60000, 20000, 16);
            T(6, 4, 3);
        }

        [Test]
        public void ShiftLeftTest()
        {
            var inputs = new[]
            {
                new { payload = (ulong)0xF, shift = 4, bits = 8, expected = (ulong)0xF0 },
                new { payload = (ulong)0xF, shift = 6, bits = 8, expected = (ulong)0xC3 },
                new { payload = (ulong)0xA, shift = 30, bits = 32, expected = (ulong)0x80000002 },
                new { payload = (ulong)0xFF, shift = 2, bits = 8, expected = (ulong)0xFF },
                new { payload = (ulong)0x12345, shift = 12, bits = 20, expected = (ulong)0x45123 },
                new { payload = (ulong)0x5555, shift = 1, bits = 16, expected = (ulong)0xAAAA },
            };

            foreach (var input in inputs)
            {
                ulong r = SimpleBitBlockCipher.ShiftLeft(input.payload, input.shift, input.bits);
                Assert.AreEqual(input.expected, r, $"expected {Convert.ToString((long)input.expected, 2)} actual: {Convert.ToString((long)r, 2)}");
            }
        }

        [Test]
        public void ShiftRightTest()
        {
            var inputs = new[]
            {
                new { payload = (ulong)0xF, shift = 4, bits = 8, expected = (ulong)0xF0 },
                new { payload = (ulong)0xF, shift = 6, bits = 8, expected = (ulong)0x3C },
                new { payload = (ulong)0xA, shift = 30, bits = 32, expected = (ulong)0x28 },
                new { payload = (ulong)0xFF, shift = 2, bits = 8, expected = (ulong)0xFF },
                new { payload = (ulong)0x12345, shift = 12, bits = 20, expected = (ulong)0x34512 },
                new { payload = (ulong)0x10, shift = 4, bits = 8, expected = (ulong)0x01 },
                new { payload = (ulong)0x08, shift = 5, bits = 8, expected = (ulong)0x40 },
                new { payload = (ulong)0x5555, shift = 1, bits = 16, expected = (ulong)0xAAAA },
            };

            foreach (var input in inputs)
            {
                ulong r = SimpleBitBlockCipher.ShiftRight(input.payload, input.shift, input.bits);
                Assert.AreEqual(input.expected, r, $"expected {input.expected:X} actual: {r:X}");
            }
        }

        [Test]
        public void OnesTest()
        {
            Assert.AreEqual(0x1F, SimpleBitBlockCipher.Ones(5));
            Assert.AreEqual(0xFF, SimpleBitBlockCipher.Ones(8));
            Assert.AreEqual(0xFFF, SimpleBitBlockCipher.Ones(12));
            Assert.AreEqual(0x3FF, SimpleBitBlockCipher.Ones(10));
            Assert.AreEqual(0x7FFFFFFFFFFFFFFF, SimpleBitBlockCipher.Ones(63));
            Assert.AreEqual(0xFFFFFFFFFFFFFFFF, SimpleBitBlockCipher.Ones(64));
        }
    }
}
