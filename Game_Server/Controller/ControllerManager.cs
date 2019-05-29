using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Reflection;    //反射

namespace Game_Server.Controller
{
    class ControllerManager
    {
        private Dictionary<RequestCode, BaseController> controllerDict = new Dictionary<RequestCode, BaseController>;

        public ControllerManager()
        {
            Init();
        }

        void Init()
        {
            DefaultController defaultController = new DefaultController();
            controllerDict.Add(defaultController.RequestCode, defaultController);
        }

        public void HandleRequest(RequestCode requestCode,ActionCode actioncode,string data)
        {
            BaseController controller;
            bool isGet = controllerDict.TryGetValue(requestCode, out controller);
            if(isGet == false)
            {
                Console.WriteLine("无法得到" + requestCode + "所对应的Controller,无法处理请求");
                return;
            }
            string methodName = Enum.GetName(typeof(ActionCode), actioncode);
            MethodInfo mi = controller.GetType().GetMethod(methodName);
            if(mi == null)
            {
                Console.WriteLine("[警告]在Controller[" + controller.GetType() + "]中没有对应的处理方法：[" + methodName + "]");
            }
            object[] parameters = new object[] { data };
            mi.Invoke(controller, parameters);
        }
    }
}
