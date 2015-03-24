using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Vanrise.Fzero.Services.WcfServiceLibrary
{
    [DataContract]
    public class generatedCall
    {
        [DataMember]
        public int ID ;
        [DataMember]
        public int SourceID ;
        [DataMember]
        public Nullable<int> MobileOperatorID ;
        [DataMember]
        public int StatusID ;
        [DataMember]
        public Nullable<int> PriorityID ;
        [DataMember]
        public int ReportingStatusID ;
        [DataMember]
        public int DurationInSeconds ;
        [DataMember]
        public Nullable<int> MobileOperatorFeedbackID ;
        [DataMember]
        public string a_number ;
        [DataMember]
        public string b_number ;
        [DataMember]
        public string CLI ;
        [DataMember]
        public string OriginationNetwork ;
        [DataMember]
        public Nullable<int> AssignedTo ;
        [DataMember]
        public Nullable<int> AssignedBy ;
        [DataMember]
        public Nullable<int> ReportID ;
        [DataMember]
        public System.DateTime AttemptDateTime ;
        [DataMember]
        public Nullable<System.DateTime> LevelOneComparisonDateTime ;
        [DataMember]
        public Nullable<System.DateTime> LevelTwoComparisonDateTime ;
        [DataMember]
        public Nullable<System.DateTime> FeedbackDateTime ;
        [DataMember]
        public Nullable<System.DateTime> AssignmentDateTime ;
        [DataMember]
        public Nullable<int> ImportID ;
        [DataMember]
        public Nullable<int> ReportingStatusChangedBy ;
        [DataMember]
        public Nullable<bool> Level1Comparison ;
        [DataMember]
        public Nullable<bool> Level2Comparison ;
        [DataMember]
        public Nullable<int> ToneFeedbackID ;
        [DataMember]
        public string FeedbackNotes ;
    }
}
       
   
