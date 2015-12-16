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
        public readonly Tuple<int, int> TokenTuple; 

        public Token(string tokenString)
        {
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                throw new PublicException("Der TokenString darf nicht null oder leer sein.");
            }

            TokenString = tokenString;

            TokenTuple = TokenValidation.ValidateTokenString(tokenString);
        }

        public string GetTokenString()
        {
            return TokenString;
        }

        public int GetElectionId()
        {
            return TokenTuple.Item1;
        }

        public int GetWahlkreisId()
        {
            return TokenTuple.Item2;
        }

        public bool IsValid()
        {
            return TokenTuple != null;
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