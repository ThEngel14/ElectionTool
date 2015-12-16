using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Service
{
    public class PublicException : Exception
    {
        public PublicException(string message) : base(message)
        {
            
        }
    }
}