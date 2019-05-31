using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Reflection;    //反射
using Game_Server.Servers;

namespace Game_Server.Controller
{
    class ControllerManager
    {
        private Dictionary<RequestCode, BaseController> controllerDict = new Dictionary<RequestCode, BaseController>();
        private Server server;

        public ControllerManager(Server server)
        {
            this.server = server;
            InitController();
        }

        void InitController()
        {
            DefaultController defaultController = new DefaultController();
            controllerDict.Add(defaultController.RequestCode, defaultController);
        }

        public void HandleRequest(RequestCode requestCode,ActionCode actioncode,string data,Client client)
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
            object[] parameters = new object[] { data,client,server };
            object o = mi.Invoke(controller, parameters);
            if (o == null || string.IsNullOrEmpty(o as string)) 
            {
                return;
            }
            server.SendResponse(client, requestCode, o as string);
        }
    }
}
