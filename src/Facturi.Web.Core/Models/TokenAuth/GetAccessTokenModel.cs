using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facturi.Models.TokenAuth
{
    public class GetAccessTokenModel
    {
        public string AccessToken { get; set; }
        public int ExpireInSeconds { get; set; }
    }
}
