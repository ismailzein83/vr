using System;

namespace TABS.Components
{
    public class UdpAppender : log4net.Appender.AppenderSkeleton
    {
        public int RemoteUdpPort { get; set; }
        public System.Net.IPAddress RemoteUdpAddress{ get; set; }
        System.Net.IPEndPoint ip;
        System.Net.Sockets.UdpClient client = new System.Net.Sockets.UdpClient();

        public override void ActivateOptions()
        {
            base.ActivateOptions();
            if (this.RemoteUdpPort <= 0) this.RemoteUdpPort = 18880;
            if (this.RemoteUdpAddress == null) this.RemoteUdpAddress = System.Net.IPAddress.Broadcast;
            ip = new System.Net.IPEndPoint(this.RemoteUdpAddress, RemoteUdpPort);
            client.Connect(ip);
        }

        protected string Escape(object msgPart)
        {
            if (msgPart == null) return string.Empty;
            return msgPart.ToString().Replace('\n', '\b');
        }

        protected override void Append(log4net.Core.LoggingEvent le)
        {
            string message = string.Format(
                "LOG4NET\n{0:yyyy-MM-dd HH:mm:ss.fff}\n{1}\n{2}\n{3}\n{4}\n{5}", 
                le.TimeStamp, 
                Environment.MachineName, 
                le.Level, 
                le.LoggerName, 
                Escape(le.MessageObject), 
                Escape(le.ExceptionObject)
            );
            
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            
            client.Send(data, data.Length);
        }

        protected override void OnClose()
        {
            client.Close();
        }
    }
}
