using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.Bypass;



namespace Vanrise.Fzero.Services.WcfServiceLibrary
{
    [ServiceContract]
    interface IFzeroService
    {
        [OperationContract]
        void Import(string filePath, string SourceName); // Import generated calls;

        [OperationContract]
        string GetSourceNameByEmail(string Email); // Get Folder of a Given Source Knowing its Email Address;

        [OperationContract]
        int GetSourceIDByEmail(string Email); // Get Folder of a Given Source Knowing its Email Address;

        [OperationContract]
        RecievedEmail GetLastEmailRecieved(int Source);

        [OperationContract]
        RecievedEmail SaveLastEmailRecieved(RecievedEmail RecievedEmail);

        [OperationContract]
        List<generatedCall> GetCallsDidNotPassLevelTwo(bool LevelTwoComparisonIsObligatory);

        [OperationContract]
        void PerformLevelTwoComparison(List<generatedCall> GeneratedCallsList);

       
    }
} 
