using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using Newtonsoft.Json;
using SOM.Main.BP.Arguments;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.Configuration;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;
using PdfSharp;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using System.Xml.XPath;
using MigraDoc.Rendering;
using System.Diagnostics;
using System.IO;

namespace BPMExtended.Main.Business
{
    public class BillingManager
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        Document document = new Document();
        MigraDoc.DocumentObjectModel.Shapes.TextFrame addressFrame;
        MigraDoc.DocumentObjectModel.Tables.Table table;
        public CustomerBalance GetCustomerBalance(string customerId)
        {
            return RatePlanMockDataGenerator.GetCustomerBalance(customerId);
        }

        public PaymentInfo SubmitToPOS(string customerId, string requestId, string ratePlanId, Guid contactId, BPMExtended.Main.Entities.OperationType operationType)
        {
            //After creating a contract with status on hold for this customer
            //Send to POS the list of services to pay with the contract id

            decimal depositAmount = 0;
            bool hasCallBaring = false;
            bool isForeigner = false;
            PaymentInfo payment = new PaymentInfo();

            RatePlanManager ratePlanManager = new RatePlanManager();
            var coreServices = ratePlanManager.GetCoreServices(ratePlanId);

            decimal amountToPay = 0;

            foreach (var service in coreServices)
            {
                amountToPay += service.SubscriptionFee;
            }

            UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
            var esqResult = new EntitySchemaQuery(connection.EntitySchemaManager, "Contact");
            esqResult.AddColumn("Name");
            esqResult.AddColumn("StCustomerDocumentType");
            esqResult.AddColumn("StSponsorDocumentIDNumber");

            // Execution of query to database and getting object with set identifier.
            var entity = esqResult.GetEntity(connection, contactId);
            object customerTypeId = entity.GetColumnValue("StCustomerDocumentTypeId");
            object sponsorNumber = entity.GetColumnValue("StSponsorDocumentIDNumber");

            //get customer type
            var esqResult2 = new EntitySchemaQuery(connection.EntitySchemaManager, "StCustomerDocumentType");
            esqResult2.AddColumn("Name");
            var entity2 = esqResult2.GetEntity(connection, customerTypeId);
            object customerType = entity2.GetColumnValue("Name");


            if (operationType == BPMExtended.Main.Entities.OperationType.TelephonyLineSubscription)
            {

                //get services
                var esqResult3 = new EntitySchemaQuery(connection.EntitySchemaManager, "StLineSubscriptionRequest");
                esqResult3.AddColumn("StServices");
                var entity3 = esqResult3.GetEntity(connection, requestId);
                object servicesJson = entity3.GetColumnValue("StServices");

                if (servicesJson.ToString() != "" && servicesJson != null && servicesJson.ToString() != "\"\"")
                {

                    List<Service> services = JsonConvert.DeserializeObject<List<Service>>(servicesJson.ToString());

                    foreach (Service service in services)
                    {
                        if (service.Id == "EE85D0BC-CE96-441A-A0FD-3179026423F5")
                        {
                            hasCallBaring = true;
                            break;
                        }
                    }

                }

            }


            if (customerType.Equals("أجنبي") && sponsorNumber.ToString().Equals(""))
            {
                if (hasCallBaring) depositAmount = 15000;
                else depositAmount = 20000;

                depositAmount = 15000;
                isForeigner = true;
            }


            //
            payment.amountToPay = amountToPay;
            payment.isForeigner = isForeigner;
            payment.depositAmount = depositAmount;

            return payment;
        }

        public decimal SubmitToPOS(string contractId, string requestId, BPMExtended.Main.Entities.OperationType operationType)
        {
            //Get from BPM the list of core services mapped to this operation type
            //Send to POS the list of services to pay with the contract id

            return 3500;
        }
        public PaymentInfo SubmitCallDetailsToPOS(string contractId, string numberOfRecords, string requestId, string customerId)
        {
            object pagesize = "";
            SysSettings.TryGetValue(this.BPM_UserConnection, "CDR_Page_Size", out pagesize);

            decimal pgsize = 0;
            decimal.TryParse(pagesize.ToString(), out pgsize);
            decimal number = 0;
            decimal.TryParse(numberOfRecords, out number);

            decimal pages = (number / pgsize);

            PaymentInfo paymentinfo = new PaymentInfo()
            {
                amountToPay = (Math.Ceiling(pages) * 1000),
            };
            return paymentinfo;
        }

        public List<BillCycle> GetBillCycles()
        {
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddYears(-2);


            List<BillCycle> billCycles = new List<BillCycle>();
            int i = 1;
            while (endDate <= startDate)
            {
                if (endDate.Month == 1 || endDate.Month == 4 || endDate.Month == 7 || endDate.Month == 10)
                {
                    string dt = endDate.Month + ", " + endDate.Year;
                    BillCycle bs = new BillCycle() { Id = i, Title = dt };
                    billCycles.Add(bs);
                    i++;
                }
                endDate = endDate.AddMonths(1);
            }
            return billCycles;
        }
        public CallDetailRequest GetCallDetails(string billCycle, string customerId, string contractId)
        {
            List<CallDetailItem> items = new List<CallDetailItem>();
            for (int i = 0; i < 100; i++)
            {
                CallDetailItem item = new CallDetailItem()
                {
                    Amount = "2",
                    Date = DateTime.Now.AddDays(-i).ToString(),
                    Duration = "2",
                    Number = "0123456",
                    Place = "Mazzeh"
                };
                items.Add(item);
            }
            CallDetailRequest request = new CallDetailRequest()
                {
                    BillCycle = billCycle,
                    CSO = "CSO",
                    Date = DateTime.Today.ToString(),
                    Name = customerId,
                    UserName = "username",
                    Items = items,
                    PhoneNumber = contractId
                };
            return request;
        }

        public bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

        public PDFDocument CreateDocument(string billCycle, string customerId, string contractId)
        {
            PDFDocument documnt = new PDFDocument();
            // Create a new MigraDoc document
            this.document = new Document();
            this.document.Info.Title = "Call Details Request";
            this.document.Info.Subject = "Call Details Request";
            this.document.Info.Author = "ST";

            CallDetailRequest request = GetCallDetails(billCycle,customerId,contractId);

            DefineStyles();

            CreatePage();

            FillContent(request);

            MigraDoc.DocumentObjectModel.Document doc = this.document;
            MigraDoc.Rendering.DocumentRenderer renderer = new DocumentRenderer(doc);
            MigraDoc.Rendering.PdfDocumentRenderer pdfRenderer = new MigraDoc.Rendering.PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            pdfRenderer.Document = doc;
            pdfRenderer.DocumentRenderer = renderer;
            pdfRenderer.RenderDocument();
            using (MemoryStream ms = new MemoryStream())
            {
                pdfRenderer.Save(ms, false);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Flush();
                ms.Read(buffer, 0, (int)ms.Length);

                //ByteArrayToFile(@"C:\BPM\test.pdf", buffer);


                EntitySchema schema = BPM_UserConnection.EntitySchemaManager.FindInstanceByName("StCDRDocuments");
                Entity entity = schema.CreateEntity(BPM_UserConnection);
                // Set the default values for the columns.
                entity.SetDefColumnValues();
                // Setting the value for the [Name] column.
                entity.SetColumnValue("StFileData", buffer);
                // Saving.
                entity.Save();
                var Id = entity.GetTypedColumnValue<string>("Id");
                


                Guid idd = new Guid(Id.ToUpper());
                // Creation of query instance with "City" root schema. 
                var esqCities = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCDRDocuments");
                esqCities.AddColumn("Id");
                esqCities.AddColumn("StFileData");

                // Creation of the first filter instance.
                var esqFirstFilter = esqCities.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", idd);

                // Adding created filters to query collection. 
                esqCities.Filters.Add(esqFirstFilter);

                // Objects, i.e. query results, filtered by two filters, will be included into this collection.
                var entities = esqCities.GetEntityCollection(BPM_UserConnection);

                if (entities.Count > 0)
                {
                    var filedat = entities[0].GetColumnValue("StFileData");//.GetTypedColumnValue<byte[]>("StFileData");
                    //ByteArrayToFile(@"C:\BPM\test.pdf", (byte[])filedat);
                     documnt = new PDFDocument() { Id = Id.ToString(), FileData = (byte[])filedat };
                }
            }
            return documnt;
        }
        void DefineStyles()
        {
            // Get the predefined style Normal.
            MigraDoc.DocumentObjectModel.Style style = this.document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";

            style = this.document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = this.document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = this.document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;

            // Create a new style called Reference based on style Normal
            style = this.document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }
        void CreatePage()
        {
            // Each MigraDoc document needs at least one section.
            Section section = this.document.AddSection();


            // Create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph();

            // Create the text frame for the address

            this.addressFrame = section.AddTextFrame();
            this.addressFrame.Height = "10.0cm";
            this.addressFrame.Width = "20.0cm";
            this.addressFrame.Left = ShapePosition.Left;
            this.addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            this.addressFrame.Top = "3.0cm";
            this.addressFrame.RelativeVertical = RelativeVertical.Page;

            // Add the print date field
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "5cm";
            paragraph.Style = "Reference";
            paragraph.AddFormattedText("CDR", TextFormat.Bold);
            //paragraph.AddTab();
            //paragraph.AddText("Cologne, ");
            //paragraph.AddDateField("dd.MM.yyyy");

            // Create the item table
            this.table = section.AddTable();
            this.table.Style = "Table";
            //this.table.Borders.Color = TableBorder;
            this.table.Borders.Width = 0.25;
            this.table.Borders.Left.Width = 0.5;
            this.table.Borders.Right.Width = 0.5;
            this.table.Rows.LeftIndent = 0;

            // Before you can add a row, you must define the columns
            MigraDoc.DocumentObjectModel.Tables.Column column = this.table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = this.table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = this.table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = this.table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = this.table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;


            // Create the header of the table
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            //row.Shading.Color = TableBlue;
            row.Cells[0].AddParagraph("Date");
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[0].MergeDown = 1;

            row.Cells[1].AddParagraph("Duration");
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].MergeDown = 1;

            row.Cells[2].AddParagraph("Place");
            row.Cells[2].Format.Font.Bold = false;
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[2].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[2].MergeDown = 1;

            row.Cells[3].AddParagraph("Number");
            row.Cells[3].Format.Font.Bold = false;
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[3].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[3].MergeDown = 1;

            row.Cells[4].AddParagraph("Amount");
            row.Cells[4].Format.Font.Bold = false;
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[4].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[4].MergeDown = 1;

            this.table.SetEdge(0, 0, 5, 1, Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 0.75, Color.Empty);
        }
        void FillContent(CallDetailRequest request)
        {
            // Fill address in address text frame
            //XPathNavigator item = SelectItem("/invoice/to");
            Paragraph paragraph = this.addressFrame.AddParagraph();
            paragraph.AddText(request.Name);
            paragraph.AddLineBreak();
            paragraph.AddText(request.PhoneNumber);
            paragraph.AddLineBreak();
            paragraph.AddText(request.UserName);
            paragraph.AddLineBreak();
            paragraph.AddText(request.CSO);
            paragraph.AddLineBreak();
            paragraph.AddText(request.BillCycle);
            paragraph.AddLineBreak();
            paragraph.AddText(request.Date);



            foreach (CallDetailItem item in request.Items)
            {

                Row row1 = this.table.AddRow();

                row1.TopPadding = 1.5;
                //row1.Cells[0].Shading.Color = TableGray;
                row1.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[0].MergeDown = 1;

                row1.Cells[1].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[1].MergeDown = 1;

                row1.Cells[2].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[2].MergeDown = 1;

                row1.Cells[3].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[3].MergeDown = 1;

                row1.Cells[4].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[4].MergeDown = 1;


                paragraph = row1.Cells[0].AddParagraph();
                paragraph.AddText(item.Date.ToString());

                paragraph = row1.Cells[1].AddParagraph();
                paragraph.AddText(item.Duration.ToString());

                paragraph = row1.Cells[2].AddParagraph();
                paragraph.AddText(item.Place.ToString());

                paragraph = row1.Cells[3].AddParagraph();
                paragraph.AddText(item.Number.ToString());

                paragraph = row1.Cells[4].AddParagraph();
                paragraph.AddText(item.Amount.ToString());

                this.table.SetEdge(0, this.table.Rows.Count - 2, 5, 1, Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 0.75);
            }



            // Add an invisible row as a space line to the table
            Row row = this.table.AddRow();
            row.Borders.Visible = false;

            // Add the total price row
            row = this.table.AddRow();
            row.Cells[0].Borders.Visible = false;
            row.Cells[0].AddParagraph("Total Price");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].MergeRight = 3;
            row.Cells[4].AddParagraph((100).ToString("0.00") + " €");

        }
        public bool ValidatePayment(string requestId)
        {
            return true;
        }

        public bool isUserNameUnique(string userName)
        {
            //TODO: call BSCS to validate the username
            return true;
        }


    }

    public class PDFDocument
    {

        public string Id { get; set; }
        public byte[] FileData { get; set; }


    }
    public class PaymentInfo
    {

        public decimal amountToPay { get; set; }
        public bool isForeigner { get; set; }
        public decimal depositAmount { get; set; }


    }
    public class Service
    {

        public string Id { get; set; }

    }
    public class BillCycle
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
    public class CallDetailRequest
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string CSO { get; set; }
        public string BillCycle { get; set; }
        public string Date { get; set; }
        public string PhoneNumber { get; set; }
        public List<CallDetailItem> Items { get; set; }
    }
    public class CallDetailItem
    {
        public string Date { get; set; }
        public string Duration { get; set; }
        public string Place { get; set; }
        public string Number { get; set; }
        public string Amount { get; set; }

    }
}
