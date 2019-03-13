using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace JwtAuthMiddleware.JwtStuff.Models
{
    [DataContract]
    internal struct JwtHeader
    {
        [DataMember]
        internal string alg { get; set; }
        [DataMember]
        internal string typ { get; set; }

        public JwtHeader(string algorithm, string type)
        {
            alg = algorithm;
            typ = type;
        }
    }
}
