using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Tamir.SharpSsh;


public partial class _Default : System.Web.UI.Page
{
 
    protected void Page_Load(object sender, EventArgs e)
    {
        //Tamir.SharpSsh.Sftp sftp = new Tamir.SharpSsh.Sftp("192.168.22.40", "root", "12345678963");
        //sftp.Connect();
        //sftp.Get("/var/spool/asterisk/monitor/OUT004470240321-20141211-054327-1418294607.40.wav", "C:\\FMS_Import\\");
        //sftp.Close();

        //Create a new SCP instance
        Scp scp = new Scp("10.10.10.53", "root", "P@ssw0rd");

        //Copy a file from local machine to remote SSH server
        scp.To("C:\\FMS_Import", "/root/Documents", true);

        //Copy a file from remote SSH server to local machine
        scp.From("C:\\FMS_Import", "/root/Documents", true);

    }


   




}