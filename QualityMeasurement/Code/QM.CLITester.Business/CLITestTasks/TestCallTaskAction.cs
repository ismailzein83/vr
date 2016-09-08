using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;
using Aspose.Cells;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using QM.CLITester.Business.CLITestTasks;
using QM.CLITester.Entities;
using Vanrise.Common.Business;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace QM.CLITester.Business
{
    public class TestCallTaskAction : SchedulerTaskAction
    {
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument,
            Dictionary<string, object> evaluatedExpressions)
        {
            TestCallTaskActionArgument testCallTaskActionArgument =
                taskActionArgument as TestCallTaskActionArgument;

            if (testCallTaskActionArgument == null)
                throw new Exception(
                    String.Format("taskActionArgument '{0}' is not of type TestCallTaskActionArgument",
                        testCallTaskActionArgument));
            if (testCallTaskActionArgument == null)
                throw new ArgumentNullException("testCallTaskActionArgument");

            TestCallManager manager = new TestCallManager();

            AddTestCallInput testCallInput = new AddTestCallInput()
            {
                CountryIds = testCallTaskActionArgument.CountryIds,
                ProfileID = testCallTaskActionArgument.ProfileID,
                SuppliersIds = testCallTaskActionArgument.SuppliersIds,
                SuppliersSourceIds = testCallTaskActionArgument.SuppliersSourceIds,
                ZoneIds = testCallTaskActionArgument.ZoneIds,
                ZoneSourceId = testCallTaskActionArgument.ZoneSourceId,
                UserId = task.OwnerId,
                ScheduleId = task.TaskId,
                Quantity = testCallTaskActionArgument.Quantity
            };

            AddTestCallOutput testCallOutput = manager.AddNewTestCall(testCallInput);
            
            TestCallTaskActionExecutionInfo exuctionInfo = new TestCallTaskActionExecutionInfo()
            {
                BatchNumber = testCallOutput.BatchNumber
            };

            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.WaitingEvent,
                ExecutionInfo = exuctionInfo
            };

            return output;
        }

        public override SchedulerTaskCheckProgressOutput CheckProgress(ISchedulerTaskCheckProgressContext context, int ownerId)
        {

            Vanrise.Security.Business.UserManager userManager = new UserManager();
            User user = userManager.GetUserbyId(ownerId);
            
            if (context.ExecutionInfo == null)
                throw new ArgumentNullException("context.ExecutionInfo");

            SchedulerTaskCheckProgressOutput output = new SchedulerTaskCheckProgressOutput()
            {
                Result = ExecuteOutputResult.Completed
            };

            TestCallManager manager = new TestCallManager();
            var testCallExecInfo = context.ExecutionInfo as TestCallTaskActionExecutionInfo;

            if (testCallExecInfo.BatchNumber != null)
            {
                List<TestCallDetail> listTestCalls = manager.GetAllbyBatchNumber(testCallExecInfo.BatchNumber.Value);
                foreach (TestCallDetail testCall in listTestCalls)
                {
                    if (testCall.Entity.CallTestStatus == CallTestStatus.GetProgressFailedWithNoRetry || testCall.Entity.CallTestStatus == CallTestStatus.InitiationFailedWithNoRetry)
                        output.Result = ExecuteOutputResult.Completed;
                    else
                    {
                        if (testCall.Entity.CallTestResult == CallTestResult.NotCompleted || testCall.Entity.CallTestResult == CallTestResult.PartiallySucceeded)
                            output.Result = ExecuteOutputResult.WaitingEvent;
                    }
                }
                int listTestCallsCount = listTestCalls.Count;
                if (output.Result == ExecuteOutputResult.Completed)
                {
                    if (listTestCallsCount > 0)
                    {
                        Workbook wbk = new Workbook();
                        wbk.Worksheets.RemoveAt("Sheet1");
                        License license = new License();
                        license.SetLicense("Aspose.Cells.lic");

                        CreateWorkSheet(wbk, "BatchNb " + testCallExecInfo.BatchNumber, listTestCalls, listTestCallsCount);
                        MemoryStream memoryStream = wbk.SaveToStream();

                        var testCallTaskArg = context.Task.TaskSettings.TaskActionArgument as TestCallTaskActionArgument;


                        SendMail(memoryStream, user.Email, testCallTaskArg.ListEmails);

                        //string filename = "D:\\" + DateTime.Now.Minute + DateTime.Now.Second + "book1.xls";
                        //wbk.Save(filename);
                    }
                }
            }
            return output;
        }

        private void CreateWorkSheet(Workbook workbook, string workSheetName, List<TestCallDetail> listTestCalls, int listTestCallsCount)
        {
            Worksheet worksheet = workbook.Worksheets.Add(workSheetName);
            
            if (listTestCallsCount > 0)
            {
                Style style = workbook.Styles[workbook.Styles.Add()];
                style.Font.Name = "Times New Roman";
                style.Font.Size = 12;
                style.Font.IsBold = true;
                style.Borders.DiagonalColor = Color.Red;
                style.Font.Color = Color.RoyalBlue;

                worksheet.Cells.SetColumnWidth(0, 4);

                SetColumnValueStyle(worksheet, style, "Id", 1);
                SetColumnValueStyle(worksheet, style, "Supplier", 2);
                SetColumnValueStyle(worksheet, style, "Country", 3);
                SetColumnValueStyle(worksheet, style, "Zone", 4);
                SetColumnValueStyle(worksheet, style, "Creation Date", 5);
                SetColumnValueStyle(worksheet, style, "PDD", 6);
                SetColumnValueStyle(worksheet, style, "Call Status", 7);
                SetColumnValueStyle(worksheet, style, "Call Result", 8);

                int rowNumber = 2;
                for (int i = 0; i < listTestCallsCount; i++)
                {
                    worksheet.Cells[rowNumber, 1].PutValue(listTestCalls[i].Entity.ID);
                    worksheet.Cells[rowNumber, 2].PutValue(listTestCalls[i].SupplierName);
                    worksheet.Cells[rowNumber, 3].PutValue(listTestCalls[i].CountryName);
                    worksheet.Cells[rowNumber, 4].PutValue(listTestCalls[i].ZoneName);
                    worksheet.Cells[rowNumber, 5].PutValue(listTestCalls[i].Entity.CreationDate.ToString("dd/MM/yyyy HH:mm:ss"));
                    worksheet.Cells[rowNumber, 6].PutValue(listTestCalls[i].Entity.Measure.Pdd);
                    worksheet.Cells[rowNumber, 7].PutValue(listTestCalls[i].CallTestStatusDescription);
                    worksheet.Cells[rowNumber, 8].PutValue(listTestCalls[i].CallTestResultDescription);
                    rowNumber++;
                }
            }
        }

        private void SetColumnValueStyle(Worksheet worksheet, Style style, string value, int row)
        {
            worksheet.Cells[1, row].PutValue(value);
            worksheet.Cells[1, row].SetStyle(style);
            worksheet.Cells.SetColumnWidth(row, 20);
        }

        private void SendMail(MemoryStream memoryStream, string userMail, string listEmails)
        {

            memoryStream.Position = 0;
            var attachment = new Attachment(memoryStream, "book1.xls");
            attachment.ContentType = new ContentType("application/vnd.ms-excel");
            attachment.TransferEncoding = TransferEncoding.Base64;
            attachment.NameEncoding = Encoding.UTF8;
            attachment.Name = "book1.xls";

            StringBuilder EmailBody = new StringBuilder();
            EmailBody.Append("<table cellspacing='0' cellpadding='0'>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 13pt; font-weight: bold'>Clear Voice</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Dear&nbsp;,</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Kindly find attached last schedule log details:</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("</table></td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>For more information, don't hesitate to contact us.</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td>&nbsp;</td></tr>");
            EmailBody.Append("<tr><td style='font-family: Arial; font-size: 11pt'>Thanks,</td></tr>");
            EmailBody.Append("<tr><td><a style='font-family: Arial; font-size: 11pt' href='http://www.vanrise.com'>www.vanrise.com</a> Team</td></tr>");
            EmailBody.Append("</table>");

            MailMessage objMail = new MailMessage();

            objMail.To.Add(userMail);

            foreach (var address in listEmails.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                objMail.To.Add(address);
            }

            string strEmailFrom = "vanrise.clitester@gmail.com";

            objMail.From = new MailAddress(strEmailFrom, "Clear Voice");
            objMail.Subject = "Clear Voice - Schedule done";
            objMail.Body = EmailBody.ToString();
            objMail.IsBodyHtml = true;
            objMail.Priority = MailPriority.High;
            objMail.Attachments.Add(attachment);
            SmtpClient smtp = new SmtpClient("smtp.gmail.com",587);
            smtp.Host = "smtp.gmail.com";
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(strEmailFrom, "passwordQ1");
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

            smtp.Send(objMail);
        }
    }
}
