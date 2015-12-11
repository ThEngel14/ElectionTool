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
        private readonly static Dictionary<Token, string> ReservedToken = new Dictionary<Token, string>(); 

        public Token BuildToken(string tokenString, string ip)
        {
            if (ip == null)
            {
                throw new Exception("Unbekannte IP-Adresse.");
            }

            var token = new Token(tokenString);
            if (!token.IsValid())
            {
                throw new Exception(string.Format("Das Token '{0}' ist ungültig.", tokenString));
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
                            "Das Token '{0}' wurde bereits genutzt. Sie dürfen es daher nicht noch einmal verwenden.", existing.TokenString));
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
                    throw new Exception("Falsche IP-Adresse festgestellt.");
                }
            }

            return token;
        }

        public void FinishedToken(Token token)
        {
            ReservedToken.Remove(token);
        }

        private static void RemoveReservedTokenForIp(string ip)
        {
            var toRemove = (from entry in ReservedToken where entry.Value.Equals(ip) select entry.Key).ToList();

            foreach (var key in toRemove)
            {
                ReservedToken.Remove(key);
            }
        }
    }
}