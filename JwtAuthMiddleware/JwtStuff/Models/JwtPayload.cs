using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace JwtAuthMiddleware.JwtStuff.Models
{
    [DataContract]
    public class JwtPayload
    {
        [DataMember]
        internal string exp { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string[] clms { get; set; }

        public JwtPayload()
        {
        }
    }
}
