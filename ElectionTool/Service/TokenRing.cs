using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Service
{
    public class TokenRing
    {
        private readonly int _length;
        private readonly bool[] _bitArray;
        private int _counter;
        private readonly int _sealBits;

        public TokenRing(int length, int sealBits)
        {
            if (length <= 0)
            {
                throw new Exception("Length must be greater than zero.");
            }

            if (sealBits < 0)
            {
                throw new Exception("SealBits must be greater than or equal to zero.");
            }

            _length = length + sealBits;
            _sealBits = sealBits;
            _bitArray = new bool[_length];
            _counter = 0;
        }

        public void AddBits(BitArray bits, int count)
        {
            if (_counter + count > _length)
            {
                throw new Exception(string.Format("BitArray is to short to copy {0} more bits.", count));
            }

            var bbits = new bool[bits.Length];
            bits.CopyTo(bbits, 0);

            Array.Copy(bbits, 0, _bitArray, _counter, count);
            _counter = (_counter +  count) % _length;
        }

        public void Rotate(int count)
        {
            // double mod because of negative count
            _counter = (((_counter + count)%_length) + _length)%_length;
        }

        public bool[] GetBits()
        {
            var temp = new bool[_length];
            // copy from _counter to end
            Array.Copy(_bitArray, _counter, temp, 0, _length - _counter);
            // copy from start to counter
            Array.Copy(_bitArray, 0, temp, _length - _counter, _counter);

            return temp;
        }

        public void AddSeal()
        {
            var b = GetBitArrayFromInt(CalculateSealNumber((_counter + _sealBits) % _length));
            AddBits(b, _sealBits);
        }

        private int CalculateSealNumber(int startIndex)
        {
            // count trues
            var countTrue = 0;
            for (var i = 0; i < _length - _sealBits; i++)
            {
                if (_bitArray[(startIndex + i) % _length])
                {
                    countTrue++;
                }
            }

            return countTrue;
        }

        public bool IsSealValid()
        {
            var expected = CalculateSealNumber(_counter);

            var sealArray = new bool[_sealBits];
            for (var i = 1; i <= _sealBits; i++)
            {
                sealArray[_sealBits - i] = _bitArray[(((_counter - i) % _length) + _length) % _length];
            }

            return expected == GetIntFromBitArray(new BitArray(sealArray));
        }

        public static int GetIntFromBitArray(BitArray bitArray)
        {
            if (bitArray.Length > 32)
            {
                throw new ArgumentException("Argument length shall be at most 32 bits.");
            }

            var array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }

        public static BitArray GetBitArrayFromInt(int i)
        {
            return new BitArray(new[] { i });
        }
    }
}