using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.AccountBalance.MainExtensions.BalancePeriod;
using Vanrise.BusinessProcess;
using Vanrise.Common.Excel;
using Vanrise.Data.SQL;
using Vanrise.ExcelConversion.Business;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Voucher.Business;
using Vanrise.Voucher.Entities;

namespace Retail.Runtime.Tasks
{
    public class SamerTask : BaseSQLDataManager, ITask
    {
        public SamerTask():base("Data Source=192.168.110.185;initial catalog=DemoProject_Dev ; User ID=development;Password=dev!123;Integrated Security=SSPI;", false)
        { }
        public void Execute()
        {




            var runtimeServices = new List<Vanrise.Runtime.Entities.RuntimeService>();
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);
            BPRegulatorRuntimeService bpRegulatorService = new BPRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationRuntimeService queueActivationRuntimeService = new QueueActivationRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };
            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };

            runtimeServices.Add(queueActivationService);
            runtimeServices.Add(schedulerService);
            runtimeServices.Add(dsRuntimeService);
            runtimeServices.Add(queueActivationRuntimeService);
            runtimeServices.Add(bpRegulatorService);
            runtimeServices.Add(queueRegulatorService);
            Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bigDataService);
            RuntimeHost host = new RuntimeHost(runtimeServices);

            //var runtimeServices = new List<Vanrise.Runtime.Entities.RuntimeService>();
            //VoucherCardsManager voucherCardManager = new VoucherCardsManager();
            //SetVoucherUsedInput input = new SetVoucherUsedInput() { PinCode = "sa", UsedBy = "mhd" };
            //SetVoucherUsedOutput output = voucherCardManager.SetVoucherUsed(input);
            //Console.ReadLine();
            // runtimeServices.Add(voucherCardManager);
            //RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            // SqlConnection cn = new SqlConnection();
            //cn.Open();
            //byte[] bytes = File.ReadAllBytes("C:\\Users\\samer.haidar\\Desktop\\SMS.xlsx");
            //var fileStream = new System.IO.MemoryStream(bytes);
            //Workbook workbook = new Workbook(fileStream);
            //Worksheet worksheet = workbook.Worksheets[0];
            //var nbOfRows = worksheet.Cells.MaxRow;
            //var nbOfCols = worksheet.Cells.MaxColumn;
            //for (int rowIndex = 1; rowIndex <= nbOfRows; rowIndex++)
            //{

            //    SMSEntity sMSEntity = new SMSEntity();
            //    var cell = worksheet.Cells[rowIndex, 0];
            //    sMSEntity.Createdate = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToDateTime(cell.Value) : default(DateTime?);
            //    cell = worksheet.Cells[rowIndex, 1];
            //    sMSEntity.Message = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);


            //    cell = worksheet.Cells[rowIndex, 2];
            //    sMSEntity.Proxiedmessage = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 3];
            //    sMSEntity.DSTaddressIN = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 4];
            //    sMSEntity.Vendordeliverystatus = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 5];
            //    sMSEntity.SRCaddressIN = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 6];
            //    sMSEntity.Attempt = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToInt32(cell.Value) : default(int?);
            //    cell = worksheet.Cells[rowIndex, 7];
            //    sMSEntity.Balancerhost = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 8];
            //    sMSEntity.Clientrequestdate = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToDateTime(cell.Value) : default(DateTime?);
            //    cell = worksheet.Cells[rowIndex, 9];
            //    sMSEntity.Clientresptime = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToDecimal(cell.Value) : default(Decimal?);
            //    cell = worksheet.Cells[rowIndex, 10];
            //    sMSEntity.Customer = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 11];
            //    sMSEntity.Customeramount = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToDecimal(cell.Value) : default(Decimal?);
            //    cell = worksheet.Cells[rowIndex, 12];
            //    sMSEntity.Customerconnhost = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 13];
            //    sMSEntity.Customerconnection = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 14];
            //    sMSEntity.Customerdeliverystatus = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 15];
            //    sMSEntity.Customerprice = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToDecimal(cell.Value) : default(Decimal?);
            //    cell = worksheet.Cells[rowIndex, 16];
            //    sMSEntity.Delivered = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToInt32(cell.Value) : default(int?);
            //    cell = worksheet.Cells[rowIndex, 17];
            //    sMSEntity.Deliverydate = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToDateTime(cell.Value) : default(DateTime?);
            //    cell = worksheet.Cells[rowIndex, 18];
            //    sMSEntity.Deliverytime = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToDecimal(cell.Value) : default(Decimal?);
            //    cell = worksheet.Cells[rowIndex, 19];
            //    sMSEntity.DLRgenerated = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToInt32(cell.Value) : default(int?);
            //    cell = worksheet.Cells[rowIndex, 20];
            //    sMSEntity.DLRgenerationdate = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToDateTime(cell.Value) : default(DateTime?);
            //    cell = worksheet.Cells[rowIndex, 21];
            //    sMSEntity.DLRstatus = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToInt32(cell.Value) : default(int?);
            //    cell = worksheet.Cells[rowIndex, 22];
            //    sMSEntity.DSTaddressOUT = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 23];
            //    sMSEntity.Encoding = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 24];
            //    sMSEntity.ExtmsgID = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 25];
            //    sMSEntity.Finalattempt = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToInt32(cell.Value) : default(int?);
            //    cell = worksheet.Cells[rowIndex, 26];
            //    sMSEntity.Gatewayport = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 27];
            //    sMSEntity.Gatewayserialnumber = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 28];
            //    sMSEntity.IMSI = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 29];
            //    sMSEntity.MsgdelID = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 30];
            //    sMSEntity.MsgID = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 31];
            //    sMSEntity.Operator = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 32];
            //    sMSEntity.Partnumber = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToInt32(cell.Value) : default(int?);
            //    cell = worksheet.Cells[rowIndex, 33];
            //    sMSEntity.Partscount = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToInt32(cell.Value) : default(int?);
            //    cell = worksheet.Cells[rowIndex, 34];
            //    sMSEntity.Partskey = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 35];
            //    sMSEntity.Queuetime = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToDecimal(cell.Value) : default(Decimal?);
            //    cell = worksheet.Cells[rowIndex, 36];
            //    sMSEntity.Replydate = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToDateTime(cell.Value) : default(DateTime?);
            //    cell = worksheet.Cells[rowIndex, 37];
            //    sMSEntity.Replyreceived = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToInt32(cell.Value) : default(int?);
            //    cell = worksheet.Cells[rowIndex, 38];
            //    sMSEntity.Routgroup = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 39];
            //    sMSEntity.Servicetype = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 40];
            //    sMSEntity.Submitted = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToInt32(cell.Value) : default(int?);
            //    cell = worksheet.Cells[rowIndex, 41];
            //    sMSEntity.Uplinkrequestdate = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToDateTime(cell.Value) : default(DateTime?);
            //    cell = worksheet.Cells[rowIndex, 42];
            //    sMSEntity.Uplinkresponsedate = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToDateTime(cell.Value) : default(DateTime?);
            //    cell = worksheet.Cells[rowIndex, 43];
            //    sMSEntity.Vendor = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 44];
            //    sMSEntity.Vendoramount = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToDecimal(cell.Value) : default(Decimal?);
            //    cell = worksheet.Cells[rowIndex, 45];
            //    sMSEntity.Vendorconnhost = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 46];
            //    sMSEntity.Vendorconnport = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 47];
            //    sMSEntity.Vendorconnection = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 48];
            //    sMSEntity.Vendorcurrency = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? cell.Value.ToString() : default(string);
            //    cell = worksheet.Cells[rowIndex, 49];
            //    sMSEntity.Vendorprice = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToDecimal(cell.Value) : default(Decimal?);
            //    cell = worksheet.Cells[rowIndex, 50];
            //    sMSEntity.Vendorresptime = cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()) ? Convert.ToDecimal(cell.Value) : default(Decimal?);
            //    cell = worksheet.Cells[rowIndex, 51];
            //    BaseSQLDataManager baseSQLDataManager = new BaseSQLDataManager();




            //    base.ExecuteScalarText(insertQuery, (cmd) =>
            //    {
            //        if (sMSEntity.Createdate.HasValue)
            //            cmd.Parameters.Add(new SqlParameter("@CreatedDate", sMSEntity.Createdate.Value));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@CreatedDate", DBNull.Value));


            //        if (sMSEntity.Message != null)
            //            cmd.Parameters.Add(new SqlParameter("@Message", sMSEntity.Message));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@Message", DBNull.Value));
            //        if (sMSEntity.Proxiedmessage != null)
            //            cmd.Parameters.Add(new SqlParameter("@ProxiedMessage", sMSEntity.Proxiedmessage));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@ProxiedMessage", DBNull.Value));
            //        if (sMSEntity.DSTaddressIN != null)
            //            cmd.Parameters.Add(new SqlParameter("@DSTAddressIN", sMSEntity.DSTaddressIN));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@DSTAddressIN", DBNull.Value));
            //        if (sMSEntity.Vendordeliverystatus != null)
            //            cmd.Parameters.Add(new SqlParameter("@VendorDeliveryStatus", sMSEntity.Vendordeliverystatus));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@VendorDeliveryStatus", DBNull.Value));
            //        if (sMSEntity.SRCaddressIN != null)
            //            cmd.Parameters.Add(new SqlParameter("@SRCAddressIN", sMSEntity.SRCaddressIN));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@SRCAddressIN", DBNull.Value));


            //        if (sMSEntity.Attempt.HasValue)
            //            cmd.Parameters.Add(new SqlParameter("@Attempt", sMSEntity.Attempt.Value));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@Attempt", DBNull.Value));

            //        if (sMSEntity.Balancerhost != null)
            //            cmd.Parameters.Add(new SqlParameter("@BalancerHost", sMSEntity.Balancerhost));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@BalancerHost", DBNull.Value));


            //        if (sMSEntity.Clientrequestdate.HasValue)
            //            cmd.Parameters.Add(new SqlParameter("@ClientRequestDate", sMSEntity.Clientrequestdate.Value));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@ClientRequestDate", DBNull.Value));


            //        if (sMSEntity.Clientresptime.HasValue)
            //            cmd.Parameters.Add(new SqlParameter("@ClientRespTime", sMSEntity.Clientresptime.Value));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@ClientRespTime", DBNull.Value));

            //        cmd.Parameters.Add(new SqlParameter("@Customer", sMSEntity.Customer));

            //        if (sMSEntity.Customeramount.HasValue)
            //            cmd.Parameters.Add(new SqlParameter("@CustomerAmount", sMSEntity.Customeramount.Value));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@CustomerAmount", DBNull.Value));


            //        if (sMSEntity.Customerconnhost != null)
            //            cmd.Parameters.Add(new SqlParameter("@CustomerConnHost", sMSEntity.Customerconnhost));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@CustomerConnHost", DBNull.Value));


            //        if (sMSEntity.Customerconnection != null)
            //            cmd.Parameters.Add(new SqlParameter("@CustomerConnection", sMSEntity.Customerconnection));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@CustomerConnHost", DBNull.Value));

            //        if (sMSEntity.Customerdeliverystatus != null)
            //            cmd.Parameters.Add(new SqlParameter("@CustomerDeliveryStatus", sMSEntity.Customerdeliverystatus));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@CustomerDeliveryStatus", DBNull.Value));


            //        if (sMSEntity.Customerprice != null)
            //            cmd.Parameters.Add(new SqlParameter("@CustomerPrice", sMSEntity.Customerprice));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@CustomerPrice", DBNull.Value));

            //        if (sMSEntity.Delivered != null)
            //            cmd.Parameters.Add(new SqlParameter("@Delivered", sMSEntity.Delivered));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@Delivered", DBNull.Value));

            //        if (sMSEntity.Deliverydate != null)
            //            cmd.Parameters.Add(new SqlParameter("@DeliveryDate", sMSEntity.Deliverydate));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@DeliveryDate", DBNull.Value));


            //        if (sMSEntity.Deliverytime != null)
            //            cmd.Parameters.Add(new SqlParameter("@DeliveryTime", sMSEntity.Deliverytime));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@DeliveryTime", DBNull.Value));

            //        if (sMSEntity.DLRgenerated != null)
            //            cmd.Parameters.Add(new SqlParameter("@DLRGenerated", sMSEntity.DLRgenerated));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@DLRGenerated", DBNull.Value));

            //        if (sMSEntity.DLRgenerationdate != null)
            //            cmd.Parameters.Add(new SqlParameter("@DLRGenerationDate", sMSEntity.DLRgenerationdate));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@DLRGenerationDate", DBNull.Value));

            //        if (sMSEntity.DLRstatus != null)
            //            cmd.Parameters.Add(new SqlParameter("@DLRStatus", sMSEntity.DLRstatus));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@DLRStatus", DBNull.Value));

            //        if (sMSEntity.DSTaddressOUT != null)
            //            cmd.Parameters.Add(new SqlParameter("@DSTAddressOUT", sMSEntity.DSTaddressOUT));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@DSTAddressOUT", DBNull.Value));


            //        if (sMSEntity.Encoding != null)
            //            cmd.Parameters.Add(new SqlParameter("@Encoding", sMSEntity.Encoding));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@Encoding", DBNull.Value));

            //        if (sMSEntity.ExtmsgID != null)
            //            cmd.Parameters.Add(new SqlParameter("@ExtMsgID", sMSEntity.ExtmsgID));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@ExtMsgID", DBNull.Value));

            //        if (sMSEntity.Finalattempt != null)
            //            cmd.Parameters.Add(new SqlParameter("@FinalAttempt", sMSEntity.Finalattempt));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@FinalAttempt", DBNull.Value));


            //        if (sMSEntity.Gatewayport != null)
            //            cmd.Parameters.Add(new SqlParameter("@GatewayPort", sMSEntity.Gatewayport));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@GatewayPort", DBNull.Value));

            //        if (sMSEntity.Gatewayserialnumber != null)
            //            cmd.Parameters.Add(new SqlParameter("@GatewaySerialNumber", sMSEntity.Gatewayserialnumber));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@GatewaySerialNumber", DBNull.Value));


            //        if (sMSEntity.IMSI != null)
            //            cmd.Parameters.Add(new SqlParameter("@IMSI", sMSEntity.IMSI));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@IMSI", DBNull.Value));


            //        if (sMSEntity.MsgdelID != null)
            //            cmd.Parameters.Add(new SqlParameter("@MsgDelID", sMSEntity.MsgdelID));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@MsgDelID", DBNull.Value));
            //        if (sMSEntity.MsgID != null)
            //            cmd.Parameters.Add(new SqlParameter("@MsgID", sMSEntity.MsgID));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@MsgID", DBNull.Value));


            //        if (sMSEntity.Operator != null)
            //            cmd.Parameters.Add(new SqlParameter("@Operator", sMSEntity.Operator));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@Operator", DBNull.Value));


            //        if (sMSEntity.Partnumber != null)
            //            cmd.Parameters.Add(new SqlParameter("@PartNumber", sMSEntity.Partnumber));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@PartNumber", DBNull.Value));

            //        if (sMSEntity.Partscount != null)
            //            cmd.Parameters.Add(new SqlParameter("@PartsCount", sMSEntity.Partscount));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@PartsCount", DBNull.Value));


            //        if (sMSEntity.Partskey != null)
            //            cmd.Parameters.Add(new SqlParameter("@Partskey", sMSEntity.Partskey));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@Partskey", DBNull.Value));


            //        if (sMSEntity.Queuetime != null)
            //            cmd.Parameters.Add(new SqlParameter("@QueueTime", sMSEntity.Queuetime));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@QueueTime", DBNull.Value));
            //        if (sMSEntity.Replydate != null)
            //            cmd.Parameters.Add(new SqlParameter("@ReplyDate", sMSEntity.Replydate));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@ReplyDate", DBNull.Value));


            //        if (sMSEntity.Replyreceived != null)
            //            cmd.Parameters.Add(new SqlParameter("@ReplyReceived", sMSEntity.Replyreceived));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@ReplyReceived", DBNull.Value));




            //        if (sMSEntity.Routgroup != null)
            //            cmd.Parameters.Add(new SqlParameter("@RoutGroup", sMSEntity.Routgroup));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@RoutGroup", DBNull.Value));

            //        if (sMSEntity.Servicetype != null)
            //            cmd.Parameters.Add(new SqlParameter("@ServiceType", sMSEntity.Servicetype));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@ServiceType", DBNull.Value));

            //        if (sMSEntity.Submitted != null)
            //            cmd.Parameters.Add(new SqlParameter("@Submitted", sMSEntity.Submitted));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@Submitted", DBNull.Value));

            //        if (sMSEntity.Uplinkrequestdate != null)
            //            cmd.Parameters.Add(new SqlParameter("@UplinkRequestDate", sMSEntity.Uplinkrequestdate));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@UplinkRequestDate", DBNull.Value));

            //        if (sMSEntity.Uplinkresponsedate != null)
            //            cmd.Parameters.Add(new SqlParameter("@UplinkResponseDate", sMSEntity.Uplinkresponsedate));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@UplinkResponseDate", DBNull.Value));

            //        if (sMSEntity.Vendor != null)
            //            cmd.Parameters.Add(new SqlParameter("@Vendor", sMSEntity.Vendor));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@Vendor", DBNull.Value));

            //        if (sMSEntity.Vendoramount != null)
            //            cmd.Parameters.Add(new SqlParameter("@VendorAmount", sMSEntity.Vendoramount));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@VendorAmount", DBNull.Value));

            //        if (sMSEntity.Vendorconnhost != null)
            //            cmd.Parameters.Add(new SqlParameter("@VendorConnHost", sMSEntity.Vendorconnhost));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@VendorConnHost", DBNull.Value));

            //        if (sMSEntity.Vendorconnport != null)
            //            cmd.Parameters.Add(new SqlParameter("@VendorConnPort", sMSEntity.Vendorconnport));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@VendorConnPort", DBNull.Value));

            //        if (sMSEntity.Vendorconnection != null)
            //            cmd.Parameters.Add(new SqlParameter("@VendorConnection", sMSEntity.Vendorconnection));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@VendorConnection", DBNull.Value));

            //        if (sMSEntity.Vendorcurrency != null)
            //            cmd.Parameters.Add(new SqlParameter("@VendorCurrency", sMSEntity.Vendorcurrency));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@VendorCurrency", DBNull.Value));

            //        if (sMSEntity.Vendorprice != null)
            //            cmd.Parameters.Add(new SqlParameter("@VendorPrice", sMSEntity.Vendorprice));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@VendorPrice", DBNull.Value));

            //        if (sMSEntity.Vendorresptime != null)
            //            cmd.Parameters.Add(new SqlParameter("@VendorRespTime", sMSEntity.Vendorresptime));
            //        else
            //            cmd.Parameters.Add(new SqlParameter("@VendorRespTime", DBNull.Value));
            //    });
            //}
        }
        public class SMSEntity
        {
            public DateTime? Createdate { get; set; }
            public string Message { get; set; }
            public string Proxiedmessage { get; set; }
            public string DSTaddressIN { get; set; }
            public string Vendordeliverystatus { get; set; }
            public string SRCaddressIN { get; set; }
            public int? Attempt { get; set; }
            public string Balancerhost { get; set; }
            public DateTime? Clientrequestdate { get; set; }
            public decimal? Clientresptime { get; set; }
            public string Customer { get; set; }
            public decimal? Customeramount { get; set; }
            public string Customerconnhost { get; set; }
            public string Customerconnection { get; set; }
            public string Customerdeliverystatus { get; set; }
            public decimal? Customerprice { get; set; }
            public int? Delivered { get; set; }
            public DateTime? Deliverydate { get; set; }
            public decimal? Deliverytime { get; set; }
            public int? DLRgenerated { get; set; }
            public DateTime? DLRgenerationdate { get; set; }
            public int? DLRstatus { get; set; }
            public string DSTaddressOUT { get; set; }
            public string Encoding { get; set; }
            public string ExtmsgID { get; set; }
            public int? Finalattempt { get; set; }
            public string Gatewayport { get; set; }
            public string Gatewayserialnumber { get; set; }
            public string IMSI { get; set; }
            public string MsgdelID { get; set; }
            public string MsgID { get; set; }
            public string Operator { get; set; }
            public int? Partnumber { get; set; }
            public int? Partscount { get; set; }
            public string Partskey { get; set; }
            public decimal? Queuetime { get; set; }
            public DateTime? Replydate { get; set; }
            public int? Replyreceived { get; set; }
            public string Routgroup { get; set; }
            public string Servicetype { get; set; }
            public int? Submitted { get; set; }
            public DateTime? Uplinkrequestdate { get; set; }
            public DateTime? Uplinkresponsedate { get; set; }
            public string Vendor { get; set; }
            public decimal? Vendoramount { get; set; }
            public string Vendorconnhost { get; set; }
            public string Vendorconnport { get; set; }
            public string Vendorconnection { get; set; }
            public string Vendorcurrency { get; set; }
            public decimal? Vendorprice { get; set; }
            public decimal? Vendorresptime { get; set; }
        }
    }
}
    
