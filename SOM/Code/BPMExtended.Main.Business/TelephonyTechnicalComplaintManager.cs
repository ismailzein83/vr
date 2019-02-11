using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;
namespace BPMExtended.Main.Business
{
    public class TelephonyTechnicalComplaintManager
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        public TechnicalDetails GetLineDescription(string Id)
        {
            Guid idd = new Guid(Id.ToUpper());
            TechnicalDetails technicalDetails = null;

            var esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StTelephonyTechnicalComplaints");
            esq.AddColumn("StReservedSwitchName");
            esq.AddColumn("StReservedSwitchID");
            esq.AddColumn("StReservedSwitchOMC");
            esq.AddColumn("StReservedVerticalMDF");
            esq.AddColumn("StReservedMDFPort");
            esq.AddColumn("StReservedCabinet");
            esq.AddColumn("StReservedCabinetPrimaryPort");
            esq.AddColumn("StReservedCabinetSecondaryPort");
            esq.AddColumn("StReservedDP");
            esq.AddColumn("StReservedDPPort");
            esq.AddColumn("StReservedDPID");
            esq.AddColumn("StMSANEID");
            esq.AddColumn("StMSANTID");
            esq.AddColumn("StMSANType");
            esq.AddColumn("StTransmitter");
            esq.AddColumn("StTransmitterPort");
            esq.AddColumn("StReceiver");
            esq.AddColumn("StReceiverPort");
            esq.AddColumn("StDSlam");
            esq.AddColumn("StDSlamOMC");
            esq.AddColumn("StDSlamPort");

            var esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idd);
            esq.Filters.Add(esqFirstFilter);

            var entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                var switchName = entities[0].GetColumnValue("StReservedSwitchName");
                var switchId = entities[0].GetColumnValue("StReservedSwitchID");
                var switchOMC = entities[0].GetColumnValue("StReservedSwitchOMC");
                var vericalMdf = entities[0].GetColumnValue("StReservedVerticalMDF");
                var mdfPort = entities[0].GetColumnValue("StReservedMDFPort");
                var cabinet = entities[0].GetColumnValue("StReservedCabinet");
                var cabinetPrimaryPort = entities[0].GetColumnValue("StReservedCabinetPrimaryPort");
                var cabinetSecondaryPort = entities[0].GetColumnValue("StReservedCabinetSecondaryPort");
                var dp = entities[0].GetColumnValue("StReservedDP");
                var dpPort = entities[0].GetColumnValue("StReservedDPPort");
                var dpId = entities[0].GetColumnValue("StReservedDPID");
                var MSANEId = entities[0].GetColumnValue("StMSANEID");
                var MSANTId = entities[0].GetColumnValue("StMSANTID");
                var MSANType = entities[0].GetColumnValue("StMSANType");
                var transmitter = entities[0].GetColumnValue("StTransmitter");
                var transmitterPort = entities[0].GetColumnValue("StTransmitterPort");
                var receiver = entities[0].GetColumnValue("StReceiver");
                var receiverPort = entities[0].GetColumnValue("StReceiverPort");
                var DSlam = entities[0].GetColumnValue("StDSlam");
                var DSlamOMC = entities[0].GetColumnValue("StDSlamOMC");
                var DSlamPort = entities[0].GetColumnValue("StDSlamPort");

                technicalDetails = new TechnicalDetails()
                {
                    Switch = switchName.ToString(),
                    SwitchId = switchId.ToString(),
                    SwitchOMC = switchOMC.ToString(),
                    VerticalMDF = vericalMdf.ToString(),
                    MDFPort = mdfPort.ToString(),
                    Cabinet= cabinet.ToString(),
                    CabinetPrimaryPort= cabinetPrimaryPort.ToString(),
                    CabinetSecondaryPort= cabinetSecondaryPort.ToString(),
                    DP= dp.ToString(),
                    DPPorts= (List<string>)dpPort,
                    DPId= dpId.ToString(),
                    MSAN_EID= MSANEId.ToString(),
                    MSAN_TID= MSANTId.ToString(),
                    MSANType= MSANType.ToString(),
                    Transmitter= transmitter.ToString(),
                    TransmitterPort= transmitterPort.ToString(),
                    Receiver= receiver.ToString(),
                    ReceiverPort= receiverPort.ToString(),
                    DSlam= DSlam.ToString(),
                    DSlamOMC= DSlamOMC.ToString(),
                    DSlamPort= DSlamPort.ToString(),
                };
            }
            return technicalDetails;
        }

    }
}
