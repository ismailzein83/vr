using System;
using System.Collections.Generic;

namespace TABS.Components
{
    public class Console : IDisposable
    {
        public enum Command
        {
            None = -1,
            Exit = 0,
            Help = 100,
            Status = 200,
            Tasks = 300,
        }

        public enum ErrorCode
        {
            Ok = 200,
            Bad_Command = 400,
            Internal_Error = 500
        }

        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(Console));

        public System.Web.HttpRequest Request { get; protected set; }
        public System.Web.HttpResponse Response { get; protected set; }

        public System.IO.Stream InputStream { get { return Request.InputStream; } }
        public System.IO.Stream OutputStream { get { return Response.OutputStream; } }

        public static Dictionary<System.Threading.Thread, Console> ActiveConsoles = new Dictionary<System.Threading.Thread, Console>();

        protected Console(System.Web.HttpRequest request, System.Web.HttpResponse response)
        {
            this.Request = request;
            this.Response = response;
            response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
            Response.Clear();
            Response.ClearHeaders();
            Response.Buffer = false;
            Response.BufferOutput = false;
            Response.DisableKernelCache();
        }

        protected void Send(ErrorCode code)
        {
            Send(string.Format("{0}\t{1}", (int)code, code.ToString()));
        }

        protected void SendEnd()
        {
            Send("\r\n\r\n");
        }

        protected void Run()
        {
            // Send Ok to Continue
            Send(ErrorCode.Ok);
            SendEnd();

            // Wait for commands
            Command command = Command.None;

            DateTime lastTimeRecieved = DateTime.Now;
            TimeSpan timeOut = TimeSpan.FromMinutes(5);

            while (command != Command.Exit && DateTime.Now.Subtract(lastTimeRecieved) < timeOut)
            {
                string line = ReadLine();
                if (line != null)
                {
                    lastTimeRecieved = DateTime.Now;
                    line = line.Trim();
                    if (!string.IsNullOrEmpty(line))
                    {
                        string commandText = line.Split(' ')[0];
                        try
                        {
                            command = (Command)Enum.Parse(typeof(Command), commandText);
                            switch (command)
                            {
                                case Command.Help:
                                    Send(@"Available Commands:\r\n- Exit\r\n- Help\r\n- Tasks\r\n- Status");
                                    SendEnd();
                                    break;
                                case Command.Tasks:
                                    Send(@"Running Tasks....");
                                    SendEnd();
                                    break;
                                default:
                                    Send(@"Unsported Command: " + command.ToString());
                                    SendEnd();
                                    break;
                            }
                        }
                        catch
                        {
                            Send(ErrorCode.Bad_Command);
                            SendEnd();
                        }
                    }
                }
            }
        }

        protected string ReadLine()
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(InputStream);
            string line = reader.ReadLine();
            return line;
        }

        public void Send(string message)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
            OutputStream.Write(buffer, 0, buffer.Length);
        }


        public static void Handle(System.Web.HttpRequest request, System.Web.HttpResponse response)
        {
            using (Console created = new Console(request, response))
            {
                try
                {
                    ActiveConsoles[System.Threading.Thread.CurrentThread] = created;
                    created.Run();
                }
                catch (Exception ex)
                {
                    log.Error("Error Running Console", ex);
                }
            }
            ActiveConsoles[System.Threading.Thread.CurrentThread] = null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if (this.Response != null)
                {
                    this.Response.Flush();
                    this.Response.End();
                }
                if (this.Request != null && this.Request.InputStream != null)
                {
                    this.Request.InputStream.Close();
                    this.Request.InputStream.Dispose();
                }
            }
            catch (Exception ex)
            {
                log.Error("Error Disposing Console", ex);
            }
        }


        #endregion

    }
}
