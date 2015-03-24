using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;


public partial class _Default : System.Web.UI.Page
{
 
    protected void Page_Load(object sender, EventArgs e)
    {
        Tamir.SharpSsh.Sftp sftp = new Tamir.SharpSsh.Sftp("192.168.22.40", "root", "12345678963");
        sftp.Connect();
        sftp.Get("/var/spool/asterisk/monitor/OUT004470240321-20141211-054327-1418294607.40.wav", "C:\\FMS_Import\\");
        sftp.Close();
    }


   




}