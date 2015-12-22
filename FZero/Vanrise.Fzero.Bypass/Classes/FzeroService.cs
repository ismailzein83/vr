using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.Bypass;


namespace Vanrise.Fzero.Bypass
{
    public class FzeroService 
    {
        public void Import(string filePath, string SourceName) // Import Calls
         {
             DataTable dt = null;
              if (filePath.Contains(".xls"))
                 {
                      dt = GeneratedCall.GetDataFromExcel(filePath, SourceName);
                 }

              else if (filePath.Contains(".xlsx"))
                 {
                      dt = GeneratedCall.GetDataFromExcel(filePath, SourceName);
                 }

             
              else if (filePath.Contains(".xml"))
                  {
                      dt = GeneratedCall.GetDataFromXml(filePath, SourceName);
                  }
            if(dt != null)
            {
                GeneratedCall.Confirm(SourceName, dt, null);
            }
              
            
        }

        public string  GetSourceNameByEmail(string Email) // Get Folder of a Given Source Knowing its Email Address;
        {
            return Source.GetByEmail(Email).Name;
        }

        public int GetSourceIDByEmail(string Email) // Get Folder of a Given Source Knowing its Email Address;
        {
            return Source.GetByEmail(Email).ID;
        }

        public RecievedEmail GetLastEmailRecieved(int Source) // Get Last Recieved Email;
        {
            return RecievedEmail.GetLast(Source);
        }

        public RecievedEmail SaveLastEmailRecieved(RecievedEmail RecievedEmail)
        {
            return RecievedEmail.Save(RecievedEmail);
        }

        public List<GeneratedCall> GetCallsDidNotPassLevelTwo(bool LevelTwoComparisonIsObligatory)
        {
            return GeneratedCall.GetCallsDidNotPassLevelTwo(LevelTwoComparisonIsObligatory);
        }



    }
}
