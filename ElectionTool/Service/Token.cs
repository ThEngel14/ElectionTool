using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElectionTool.Entity_Framework;

namespace ElectionTool.Service
{
    public class Token
    {
        public readonly string TokenString;
        public readonly int ElectionId;
        public readonly int WahlkreisId;

        public Token(string tokenString)
        {
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                throw new Exception("The tokenString cannot be null or white space.");
            }

            TokenString = tokenString;

            if (TokenString.Contains('-'))
            {
                var s = tokenString.Split('-');

                int.TryParse(s[0], out ElectionId);
                int.TryParse(s[1], out WahlkreisId);
            }
            else
            {
                ElectionId = -1;
                WahlkreisId = -1;
            }
        }

        public string GetTokenString()
        {
            return TokenString;
        }

        public int GetElectionId()
        {
            return ElectionId;
        }

        public int GetWahlkreisId()
        {
            return WahlkreisId;
        }

        public bool IsValid()
        {
            return 0 < ElectionId && ElectionId < 3 && 0 < WahlkreisId && WahlkreisId < 300;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (!(obj is Token))
            {
                return false;
            }

            return TokenString.Equals(((Token)obj).GetTokenString());
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return TokenString.GetHashCode();
        }
    }
}