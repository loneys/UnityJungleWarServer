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
        RequestCode requestCod = RequestCode.None;

        public virtual void DefaultHandle() { }
    }
}
