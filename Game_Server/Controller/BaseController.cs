using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Game_Server.Controller
{
    abstract class BaseController
    {

        RequestCode requestCode = RequestCode.None;

        public RequestCode RequestCode
        {
            get
            {
                return requestCode;
            }
        }

        public virtual string DefaultHandle(string data) { return null; }
    }
}
