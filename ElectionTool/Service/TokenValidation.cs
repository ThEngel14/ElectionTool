using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElectionTool.Entity_Framework;

namespace ElectionTool.Service
{
    public class TokenValidation
    {
        private static int _counter = new Random().Next();

        private static readonly Random Rnd = new Random();

        private static readonly char[] CharArray =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',
            'W', 'X', 'Y', 'Z'
        };

        private static int GetValueOfChar(char c, int offset)
        {
            var index = CharArray.ToList().IndexOf(c);
            return (((index - offset) % 26) + 26) % 26;
        }

        public static string GenerateTokenString(int electionId, int wahlkreisId)
        {
            var ring = new TokenRing(47, 6);
            ring.AddBits(TokenRing.GetBitArrayFromInt(_counter++), 31);
            ring.AddBits(TokenRing.GetBitArrayFromInt(electionId), 7);
            ring.AddBits(TokenRing.GetBitArrayFromInt(wahlkreisId), 9);
            ring.AddSeal();

            var offset = Rnd.Next() % 64;
            var offsetChar = Rnd.Next() % 32;

            ring.Rotate(offset);

            var finalRing = new TokenRing(64, 0);
            finalRing.AddBits(TokenRing.GetBitArrayFromInt(offsetChar), 5);
            finalRing.AddBits(TokenRing.GetBitArrayFromInt(offset), 6);
            finalRing.AddBits(new BitArray(ring.GetBits()), 53);

            var part1 = "";
            var part2 = "";
            var part3 = "";
            var part4 = "";

            var finalBits = finalRing.GetBits();

            var anz = 0;
            for (var i = 0; i < 64; i += 8)
            {
                var byteRange = new bool[8];
                Array.Copy(finalBits, i, byteRange, 0, 8);

                var allInt = TokenRing.GetIntFromBitArray(new BitArray(byteRange));

                var number = allInt/26;
                var numberChar = ((allInt % 26) + (i == 0 ? 0 : offsetChar)) % 26;
                var resultChar = CharArray[numberChar];

                if (anz++ < 4)
                {
                    part1 += number;
                    part2 += resultChar;
                }
                else
                {
                    part3 += number;
                    part4 += resultChar;
                }
            }

            return string.Format("{0}-{1}-{2}-{3}", part1, part2, part3, part4);
        }

        public static Tuple<int, int> ValidateTokenString(string value)
        {
            var charOffset = 0;

            try
            {
                var tokenString = value.ToUpper();
                var splittedTokenString = tokenString.Split('-');

                // 4 parts
                if (splittedTokenString.Length != 4)
                {
                    throw new Exception();
                }

                // each part has 4 characters
                for (var i = 0; i < 4; i++)
                {
                    if (splittedTokenString[i].Length != 4)
                    {
                        throw new Exception();
                    }
                }

                // revert code to ints
                var bytes = new int[8];
                for (var i = 0; i < 2; i++)
                {
                    var nstring = splittedTokenString[2*i];
                    var cstring = splittedTokenString[2*i + 1];

                    for (var k = 0; k < 4; k++)
                    {
                        var number = int.Parse(nstring[k].ToString());
                        var character = cstring[k];

                        var charValue = GetValueOfChar(character, charOffset);

                        bytes[i*4+k] = number*26 + charValue;
                        if (i == 0 && k == 0)
                        {
                            // calculate char offset (first 5 bits of first value in bytes
                            var b = TokenRing.GetBitArrayFromInt(bytes[0]);
                            var bbool = new bool[32];
                            b.CopyTo(bbool, 0);

                            var bits5 = new bool[5];
                            Array.Copy(bbool, 0, bits5, 0, 5);

                            charOffset = TokenRing.GetIntFromBitArray(new BitArray(bits5));
                        }
                    }
                }

                //revert ints to bits
                var completeRing = new TokenRing(64, 0);
                for (var i = 0; i < 8; i++)
                {
                    completeRing.AddBits(TokenRing.GetBitArrayFromInt(bytes[i]), 8);
                }

                //calculate offset
                var allBits = completeRing.GetBits();
                var offsetBits = new bool[6];
                Array.Copy(allBits, 5, offsetBits, 0, 6);
                var offset = TokenRing.GetIntFromBitArray(new BitArray(offsetBits));

                // create ring
                var ringBits = new bool[53];
                Array.Copy(allBits, 11, ringBits, 0, 53);
                var rotateRing = new TokenRing(47, 6);
                rotateRing.AddBits(new BitArray(ringBits), 53);

                //rotate ring
                rotateRing.Rotate(-offset);

                //check validity
                if (!rotateRing.IsSealValid())
                {
                    throw new Exception();
                }

                var baseBits = rotateRing.GetBits();

                // electionId
                var electionBits = new bool[7];
                Array.Copy(baseBits, 31, electionBits, 0, 7);
                var electionId = TokenRing.GetIntFromBitArray(new BitArray(electionBits));

                // wahlkreisId
                var wahlkreisBits = new bool[9];
                Array.Copy(baseBits, 38, wahlkreisBits, 0, 9);
                var wahlkreisId = TokenRing.GetIntFromBitArray(new BitArray(wahlkreisBits));

                // check if electionId and wahlkreisId exists
                using(var context = new ElectionDBEntities())
                {
                    var election = context.Elections.SingleOrDefault(e => e.Id == electionId);
                    var wahlkreis = context.Wahlkreis.SingleOrDefault(w => w.Id == wahlkreisId);

                    if (election == null || wahlkreis == null)
                    {
                        throw new Exception();
                    }

                    return Tuple.Create(electionId, wahlkreisId);
                }
            }
            catch (Exception)
            {
                return null;
            }
        } 
    }
}