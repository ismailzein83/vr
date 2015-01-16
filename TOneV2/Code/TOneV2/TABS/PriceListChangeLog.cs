using System;
using System.Collections.Generic;

namespace TABS
{
    public abstract class PriceListChangeLog
    {
        public enum ChangeTypes { Unknown = -1, New = 0, Close = 1 }

        public int ID { get; protected set; }
        public DateTime Date { get; protected set; }
        public abstract long ObjectID { get; }
        public PriceList PriceList { get; protected set; }
        public ChangeTypes ChangeType { get; protected set; }
        public virtual Interfaces.IDateTimeEffective Entity { get; protected set; }
        public abstract string ObjectType { get; }
        public abstract string Description { get; }
        public DateTime? BeginEffectiveDate { get { return Entity.BeginEffectiveDate; } }
        public DateTime? EndEffectiveDate { get { return Entity.EndEffectiveDate; } }

        protected PriceListChangeLog() {}

        protected PriceListChangeLog(Interfaces.IDateTimeEffective entity, PriceList priceList)
        {
            this.Date = DateTime.Now;
            this.Entity = entity;
            this.PriceList = priceList;
            this.ChangeType = entity.EndEffectiveDate == null ? ChangeTypes.New : ChangeTypes.Close;
        }

        public static List<PriceListChangeLog> CreateChangeLogs(PersistensePriorityList objectsToSave, PriceList priceList)
        {
            List<TABS.PriceListChangeLog> logEntries = new List<TABS.PriceListChangeLog>();
            foreach (object o in objectsToSave)
            {
                TABS.PriceListChangeLog log = TABS.PriceListChangeLog.CreateChangeLog(o, priceList);
                if (log != null) logEntries.Add(log);
            }
            return logEntries;
        }

        public static PriceListChangeLog CreateChangeLog(object logged, PriceList priceList)
        {
            try
            {
                PriceListChangeLog changeLog = null;

                if (logged is TABS.Zone)
                {
                    changeLog = new PriceListZoneChangeLog((Zone)logged, priceList);
                }
                else if (logged is TABS.Code)
                {
                    changeLog = new PriceListCodeChangeLog((Code)logged, priceList);
                }
                else if (logged is TABS.Rate)
                {
                    changeLog = new PriceListRateChangeLog((Rate)logged, priceList);
                }
                else
                {
                    return null;
                }
                return changeLog;
            }
            catch
            {
                return null;
            }
        }
    }

    public class PriceListZoneChangeLog : PriceListChangeLog
    {
        protected PriceListZoneChangeLog() { }
        public Zone Zone { get { return (Zone)Entity; } set { Entity = value; } }
        public PriceListZoneChangeLog(Zone zone, PriceList priceList) : base(zone, priceList) { }
        public override string Description { get { return Zone.Name; } }
        public override long ObjectID { get { return Zone.ZoneID; } }
        public override string ObjectType { get { return "Z"; } } 
    }

    public class PriceListCodeChangeLog : PriceListChangeLog
    {
        protected PriceListCodeChangeLog() { }
        public Code Code { get { return (Code)Entity; } set { Entity = value; } }
        public PriceListCodeChangeLog(Code code, PriceList priceList) : base(code, priceList) { }
        public override string Description { get { return string.Format("{0}, {1}", Code.Zone.Name, Code.Value); } }
        public override long ObjectID { get { return Code.ID; } }
        public override string ObjectType { get { return "C"; } } 
    }

    public class PriceListRateChangeLog : PriceListChangeLog
    {
        protected PriceListRateChangeLog() { }
        public Rate Rate { get { return (Rate)Entity; } set { Entity = value; } }
        public PriceListRateChangeLog(Rate rate, PriceList priceList) : base(rate, priceList) { }
        public override string Description { get { return string.Format("{0}, {1}", Rate.Zone.Name, Rate.Value.ToString()); } }
        public override long ObjectID { get { return Rate.ID; } }
        public override string ObjectType { get { return "R"; } } 
    }
}