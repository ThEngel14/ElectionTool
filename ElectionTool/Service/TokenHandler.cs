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
                throw new Exception("Unknown IP-Address.");
            }

            var delaySeconds = CheckHandleRequest(tokenString, ip);
            if (delaySeconds > 0)
            {
                // to many invalid requests. Used to handle brute force attacks  
                throw new PublicException(
                    string.Format(
                        "Sie haben bereits zu viele ungültige Token abgegeben.\n" +
                        "Sie können ein neues Token erst wieder in {0} Sekunden ({1:hh:mm:ss}) abgeben.",
                        delaySeconds, DateTime.Now.AddSeconds(delaySeconds)));
            }

            var token = new Token(tokenString);
            if (!token.IsValid())
            {
                // save invalid token request
                using (var context = new ElectionDBEntities())
                {
                    context.InvalidTokenRequests.Add(new InvalidTokenRequest
                    {
                        IP = ip,
                        Token = tokenString,
                        Timestamp = DateTime.Now
                    });
                    context.SaveChanges();
                }
                throw new PublicException(string.Format("Das Token '{0}' ist ungültig.", tokenString));
            }

            if (!ReservedToken.ContainsKey(token))
            {
                using (var context = new ElectionDBEntities())
                {
                    var existing = context.UsedTokens.SingleOrDefault(t => t.TokenString.Equals(tokenString));
                    // new token requested
                    if (existing != null)
                    {
                        throw new PublicException(string.Format(
                            "Das Token '{0}' wurde bereits genutzt. Sie dürfen es daher nicht noch einmal verwenden.", existing.TokenString));
                    }

                    // add new token to database
                    context.UsedTokens.Add(new UsedToken
                    {
                        TokenString = tokenString
                    });
                    
                    context.SaveChanges();
                }

                ReservedToken.Add(token, ip);
            }
            else
            {
                // token for actual voting requested
                var fromIp = ReservedToken[token];
                if (!ip.Equals(fromIp))
                {
                    throw new Exception("Wrong IP-Address detected.");
                }
            }

            return token;
        }

        public void FinishedToken(Token token)
        {
            ReservedToken.Remove(token);
        }

        public int CheckHandleRequest(string tokenString, string ip)
        {
            const int minutesTimeRange = 5;
            const int invalidRequestsAllowed = 3;

            var last5Min = DateTime.Now.AddMinutes(-minutesTimeRange);

            using (var context = new ElectionDBEntities())
            {
                var invalidRequests = context.InvalidTokenRequests.Where(r => r.Timestamp >= last5Min && r.IP.Equals(ip));

                var invalidRequestCount = invalidRequests.Count();

                if (invalidRequestCount < invalidRequestsAllowed)
                {
                    return 0;
                }

                var invalidRequestLatest = invalidRequests.Max(r => r.Timestamp);
                var delaySeconds = (int) Math.Max(10, Math.Pow(2, invalidRequestCount - invalidRequestsAllowed));

                var nextTime = invalidRequestLatest.AddSeconds(delaySeconds);

                var diff = nextTime - DateTime.Now;
                return Math.Max(0, diff.Seconds + diff.Minutes*60 + diff.Hours*3600);
            }
        }
    }
}