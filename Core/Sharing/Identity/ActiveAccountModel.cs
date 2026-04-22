using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Sharing.Identity
{
    public class RestPasswordModel : TokenRequestModel
    {
        public required string Token { get; set; }
    }

}