using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace QM.CLITester.Entities
{
    public class TestCall : Vanrise.Entities.EntitySynchronization.IItem
    {
        public long ID { get; set; }
        public int SupplierID { get; set; }
        public int CountryID { get; set; }
        public long ZoneID { get; set; }
        public int UserID { get; set; }
        public int ProfileID { get; set; }
        public DateTime CreationDate { get; set; }
        public Object InitiateTestInformation { get; set; }
        public Object TestProgress { get; set; }
        public Measure Measure { get; set; }
        public CallTestStatus CallTestStatus { get; set; }
        public CallTestResult CallTestResult { get; set; }
        public int InitiationRetryCount { get; set; }
        public int GetProgressRetryCount { get; set; }
        public string FailureMessage { get; set; }
        public long BatchNumber { get; set; }
        public int ScheduleId { get; set; }
        public int Quantity { get; set; }

        long IItem.ItemId
        {
            get
            {
                return ID;
            }
            set
            {
                this.ID = (long)value;
            }
        }
    }
}
