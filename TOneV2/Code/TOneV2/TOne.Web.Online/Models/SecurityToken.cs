using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOne.Web.Online.Models
{
    public class SecurityToken
    {
        [ThreadStatic]
        static SecurityToken _current;
        public static SecurityToken Current
        {
            get
            {
                return _current;
            }
            internal set
            {
                if (_current != null)
                    throw new Exception("Current Token is already set");
                _current = value;
            }
        }

        public int  UserId { get; set; }
        public string Username { get; set; }

        public string UserDisplayName { get; set; }

        public DateTime IssuedAt { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}