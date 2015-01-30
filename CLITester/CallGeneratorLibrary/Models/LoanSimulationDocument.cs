using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CallGeneratorLibrary.Repositories;
using CallGeneratorLibrary.Utilities;

namespace CallGeneratorLibrary
{
    //public partial class LoanSimulationDocument
    //{
    //    public virtual string FilePath
    //    {
    //        get
    //        {
    //            //string folder = System.Web.HttpContext.Current.Server.MapPath(Config.ProjectsResources) + "Files/";
    //            string folder = System.Web.HttpContext.Current.Server.MapPath(Config.ProjectsResources);
    //            System.IO.FileInfo file = this.Extract(new System.IO.DirectoryInfo(folder));

    //            //return Config.ProjectsResources + "Files/" + file.Name;
    //            return Config.ProjectsResources + file.Name;
    //        }
    //    }

    //    #region Methods
    //    public FileInfo Extract(DirectoryInfo destinationAbsolutePath)
    //    {
    //        FileInfo answer = null;

    //        try
    //        {
    //            if (!destinationAbsolutePath.Exists)
    //                destinationAbsolutePath.Create();

    //            string path = destinationAbsolutePath.FullName + "\\" + FileName + Extension;

    //            //check if the file already exists, otherwise create
    //            answer = new FileInfo(path);
    //            if (!answer.Exists)
    //            {
    //                //load the image 
    //                LoanSimulationDocumentRepository.Load(this.Id);

    //                FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
    //                BinaryWriter writer = new BinaryWriter(stream);

    //                writer.Write(DocumentFile.ToArray(), 0, DocumentFile.Length);

    //                writer.Close();
    //                stream.Close();
    //            }

    //            answer = new FileInfo(FileName + Extension);
    //        }
    //        catch (System.Exception ex)
    //        {
    //            Logger.LogException(ex);
    //        }

    //        return answer;
    //    }
    //    #endregion
    //}
}
