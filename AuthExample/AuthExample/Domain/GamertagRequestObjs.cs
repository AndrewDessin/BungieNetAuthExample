using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthExample.Domain
{
    public class GamertagResponse : ResponseBase
    {

        public ResponseObj Response { get; set; }

        public class ResponseObj
        {

            public string GamerTag { get; set; }


            public string PsnId { get; set; }


        }
    }
}
