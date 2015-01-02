using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;

namespace SignalRService
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:8080";

            using (WebApp.Start(url))
            {
                Console.WriteLine("Server running on {0}", url);
//                Timer t = new Timer(TimerTick, null, 0, 1000);
                Console.ReadLine();
            }
        }

        private static void TimerTick(object state)
        {
            string message = DateTime.Now.ToLongTimeString();
            Console.WriteLine(message);
            ServiceHub.SendServerMessage("Server", message);
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }

    public class ServiceHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }

        internal static void SendServerMessage(string name, string message)
        {
            IHubContext con = GlobalHost.ConnectionManager.GetHubContext<ServiceHub>();
            con.Clients.All.addMessage(name, message);
        }

        public override Task OnConnected()
        {
            string handle = Context.ConnectionId;
            Clients.All.addMessage(handle, "connected");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string handle = Context.ConnectionId;
            Clients.All.addMessage(handle, "disconnected");
            return base.OnDisconnected(stopCalled);
        }
    }
}
