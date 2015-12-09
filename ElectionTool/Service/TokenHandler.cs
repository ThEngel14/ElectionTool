using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using ElectionTool.Entity_Framework;

namespace ElectionTool.Service
{
    public class TokenHandler
    {
        private readonly Dictionary<Token, string> ReservedToken = new Dictionary<Token, string>(); 

        public Token GenerateToken(int electionId, int wahlkreisId)
        {
            return new Token(string.Format("{0}-{1}", electionId, wahlkreisId));
        }

        public Token BuildToken(string tokenString, string ip)
        {
            if (ip == null)
            {
                throw new Exception("Unknown IP failure.");
            }

            var token = new Token(tokenString);
            if (!token.IsValid())
            {
                throw new Exception(string.Format("The Token '{0}' is not valid.", tokenString));
            }

            if (!ReservedToken.ContainsKey(token))
            {
                using (var context = new ElectionDBEntities())
                {
                    var existing = context.UsedTokens.SingleOrDefault(t => t.TokenString.Equals(tokenString));
                    // new token requested
                    if (existing != null)
                    {
                        throw new Exception(string.Format(
                            "Token '{0}' has already been used. You cannot use it again.", existing.TokenString));
                    }

                    // add new token to database
                    context.UsedTokens.Add(new UsedToken
                    {
                        TokenString = tokenString
                    });
                    
                    //TODO: Only uncommented for debugging. This has to be done!!!
                    //context.SaveChanges();
                }

                RemoveReservedTokenForIp(ip);
                ReservedToken.Add(token, ip);
            }
            else
            {
                // token for actual voting requested
                var fromIp = ReservedToken[token];
                if (!ip.Equals(fromIp))
                {
                    throw new Exception("Wrong IP Address detected.");
                }
            }

            return token;
        }

        public void FinishedToken(Token token)
        {
            ReservedToken.Remove(token);
        }

        private void RemoveReservedTokenForIp(string ip)
        {
            var toRemove = (from entry in ReservedToken where entry.Value.Equals(ip) select entry.Key).ToList();

            foreach (var key in toRemove)
            {
                ReservedToken.Remove(key);
            }
        }
    }
}