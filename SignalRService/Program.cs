using System;
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
            //URL to host the SignalR service from
            // TODO: Push URL to configuration
            const string url = "http://localhost:8080";

            //Start the WebApp and display to console
            //TODO: Convert this to run as a windows service
            using (WebApp.Start(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }
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
        // Method called by clients to broadcast messages
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }

        //NOTE: can probably be used to alert when things happen, like chnges in a db?
        /// <summary>
        /// Allows the hosting process to broadcast
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        internal static void SendServerMessage(string name, string message)
        {
            IHubContext con = GlobalHost.ConnectionManager.GetHubContext<ServiceHub>();
            //addMessage - calling a method on clients
            con.Clients.All.addMessage(name, message);
        }

        /// <summary>
        /// broadcast a message when a client connects
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            string handle = Context.ConnectionId;
            Clients.All.addMessage(handle, "connected");
            return base.OnConnected();
        }

        /// <summary>
        /// broadcast a message when a client disconnects
        /// </summary>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            string handle = Context.ConnectionId;
            Clients.All.addMessage(handle, "disconnected");
            return base.OnDisconnected(stopCalled);
        }
    }
}
