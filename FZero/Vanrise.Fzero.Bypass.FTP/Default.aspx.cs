using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Chilkat;
public partial class _Default : System.Web.UI.Page
{
 
    protected void Page_Load(object sender, EventArgs e)
    {

        //  Important: It is helpful to send the contents of the
        //  sftp.LastErrorText property when requesting support.

        Chilkat.SFtp sftp = new Chilkat.SFtp();

        //  Any string automatically begins a fully-functional 30-day trial.
        bool success = sftp.UnlockComponent("Anything for 30-day trial");
        if (success != true)
        {
            Response.Write(sftp.LastErrorText +"/n");
            return;
        }

        //  Set some timeouts, in milliseconds:
        sftp.ConnectTimeoutMs = 5000;
        sftp.IdleTimeoutMs = 10000;

        //  Connect to the SSH server.
        //  The standard SSH port = 22
        //  The hostname may be a hostname or IP address.
        int port;
        string hostname;
        hostname = "10.10.10.53";
        port = 22;
        success = sftp.Connect(hostname, port);
        if (success != true)
        {
            Response.Write(sftp.LastErrorText + "/n");
            return;
        }

        //  Authenticate with the SSH server.  Chilkat SFTP supports
        //  both password-based authenication as well as public-key
        //  authentication.  This example uses password authenication.
        success = sftp.AuthenticatePw("root", "P@ssw0rd");
        if (success != true)
        {
            Response.Write(sftp.LastErrorText + "/n");
            return;
        }

        //  After authenticating, the SFTP subsystem must be initialized:
        success = sftp.InitializeSftp();
        if (success != true)
        {
            Response.Write(sftp.LastErrorText + "/n");
            return;
        }

        //  Open a directory on the server...
        //  Paths starting with a slash are "absolute", and are relative
        //  to the root of the file system. Names starting with any other
        //  character are relative to the user's default directory (home directory).
        //  A path component of ".." refers to the parent directory,
        //  and "." refers to the current directory.
        string handle;
        handle = sftp.OpenDir("/root");
        if (handle == null)
        {
            Response.Write(sftp.LastErrorText + "/n");
            return;
        }

        //  Download the directory listing:
        Chilkat.SFtpDir dirListing = null;
        dirListing = sftp.ReadDir(handle);
        if (dirListing == null)
        {
            Response.Write(sftp.LastErrorText + "/n");
            return;
        }

        //  Iterate over the files.
        int i;
        int n = dirListing.NumFilesAndDirs;
        if (n == 0)
        {
            Response.Write("No entries found in this directory." + "/n");
        }
        else
        {
            for (i = 0; i <= n - 1; i++)
            {
                Chilkat.SFtpFile fileObj = null;
                fileObj = dirListing.GetFileObject(i);


                if (!fileObj.IsDirectory && fileObj.Filename.ToUpper().Contains(".DAT"))
                {
                    Response.Write(fileObj.Filename);
                    Response.Write(fileObj.FileType);
                    Response.Write("Size in bytes: " + Convert.ToString(fileObj.Size32));
                    Response.Write("----");
                    
                    //  Rename the file or directory:
                    success = sftp.RenameFileOrDir(fileObj.Filename, fileObj.Filename.Replace(".DAT",".old"));

                }


             

            }

        }

        //  Close the directory
        success = sftp.CloseHandle(handle);
        if (success != true)
        {
            Response.Write(sftp.LastErrorText + "/n");
            return;
        }

        Response.Write("Success." + "/n");


    }


   




}