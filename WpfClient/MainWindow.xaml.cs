using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNet.SignalR.Client;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Handle { get; set; }
        public IHubProxy HubProxy { get; set; }
        private const string URI = "http://localhost:8080/signalr";
        public HubConnection Connection { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendClick(object sender, RoutedEventArgs e)
        {
            HubProxy.Invoke("Send", Handle, TbMessage.Text);
            TbMessage.Text = string.Empty;
            TbMessage.Focus();
        }

        private void ConnectionClosed()
        {
            var dp = Application.Current.Dispatcher;
            dp.Invoke(() => ChatPanel.Visibility = Visibility.Collapsed);
            dp.Invoke(() => BtnSend.IsEnabled = false);
            dp.Invoke(() => StatLabel.Content = string.Format("Disconnected from {0}", URI));
            dp.Invoke(() => SignInPanel.Visibility = Visibility.Visible);
        }

        private async void ConnectAsync()
        {
            Connection = new HubConnection(URI);
            Connection.Closed += ConnectionClosed;
            HubProxy = Connection.CreateHubProxy("ServiceHub");

            HubProxy.On<string, string>("AddMessage", (name, message) =>
                this.Dispatcher.Invoke(() =>
                    RtConsole.AppendText(string.Format("{0}: {1}\r", name, message))));

            try
            {
                await Connection.Start();
            }
            catch (Exception)
            {
                StatLabel.Content = "Connection Failed";
                return;
            }

            SignInPanel.Visibility = Visibility.Collapsed;
            ChatPanel.Visibility = Visibility.Visible;
            BtnSend.IsEnabled = true;
            TbMessage.Focus();
            RtConsole.AppendText("Conected to " + URI + "\r");
        }

        private void SignInClick(object sender, RoutedEventArgs e)
        {
            Handle = tbUser.Text;

            if (!string.IsNullOrEmpty(Handle))
            {
                StatLabel.Visibility = Visibility.Visible;
                StatLabel.Content = string.Format("Connecting to {0} ...", URI);
                ConnectAsync();
            }
        }

        private void ClientClosing(object sender, CancelEventArgs e)
        {
            if (Connection != null)
            {
                Connection.Stop();
                Connection.Dispose();
            }
        }
    }
}
