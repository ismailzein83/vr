using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.Entities;
using Terrasoft.Core.Configuration;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System.IO;
using SOM.Main.Entities;
using BPMExtended.Main.Entities;
using BPMExtended.Main.Common;
using BPMExtended.Main.SOMAPI;

namespace BPMExtended.Main.Business
{
    public class CallDetailsManager
    {
        #region Initialization
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
        #endregion

        #region Public
        public CallDetailRequest GetCallDetails(string contractId, string year, string month, bool isNational, bool isInternational, bool isGSM, string phoneNumber, string customerName, string contactId)
        {
            List<CallDetails> callDetailsEntityList = new List<CallDetails>();
            var businessEntityManager = new BusinessEntityManager();
            int monthVal = businessEntityManager.getMonthNumberByMonthName(month);
            int yearVal = Int32.Parse(year);

            DateTime beginDate = new DateTime(yearVal, monthVal, 1).Date;
            DateTime endDate = monthVal != 12 ? new DateTime(yearVal, monthVal + 1, 1) : new DateTime(yearVal + 1, 1, 1);

            using (SOMClient client = new SOMClient())
            {
                callDetailsEntityList = client.Get<List<CallDetails>>(String.Format("api/SOM.ST/Billing/GetCallDetails?ContractId={0}&BeginDate={1}&EndDate={2}&IsNational={3}&IsInternational={4}&isGSM={5} ", contractId, beginDate, endDate, isNational, isInternational, isGSM));
               
            }
                
            var crmManager = new CRMCustomerManager();
            var cso = crmManager.GetCSOByContactIdOrAccountId(contactId, "");

            CallDetailRequest request = new CallDetailRequest()
            {
                // BillCycle = billCycle,
                CSO = cso != null ? "CSO: " + cso.Name : null,
                Date = beginDate!=null? "Issue Date: "+ beginDate.ToString("dd/MM/yyyy") :null,
                Name = contractId != null ? "Contract ID: " + contractId:null,
                UserName = customerName!=null? "Customer Name: " + customerName:null,
                Items = callDetailsEntityList,
                PhoneNumber = phoneNumber != null? "Phone Number: " + phoneNumber:null
            };

            return request;
        }
        public PDFDocument CreateDocument(string contractId, string year, string month, bool isNational, bool isInternational, bool isGSM, string phoneNumber, string customerName, string contactId)
        {
            PDFDocument documnt = new PDFDocument();
            // Create a new MigraDoc document
            this.document = new Document();
            this.document.Info.Title = "Call Details Request";
            this.document.Info.Subject = "Call Details Request";
            this.document.Info.Author = "ST";


            CallDetailRequest request = GetCallDetails(contractId, year, month, isNational, isInternational, isGSM, phoneNumber, customerName, contactId);

            var catalogManager = new CatalogManager();
            int numberOfRecordsPerPage = catalogManager.GetNumberOfRecordsPerPage();
            int normalizedNumberOfRecordsPerPage = numberOfRecordsPerPage <= 0 || numberOfRecordsPerPage > 22 ? 22 : numberOfRecordsPerPage;

            DefineStyles();
            CreatePages(request, normalizedNumberOfRecordsPerPage);

            //FillContent(request);

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
                    documnt = new PDFDocument() { Id = Id.ToString(), FileData = (byte[])filedat, pageCount = pdfRenderer.PageCount };
                }
            }
            return documnt;
        }
        public byte[] GetFileDataByFileId(string fileId)
        {

            Guid idd = new Guid(fileId.ToUpper());
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
            if (entities.Count == 0)
            {
                return null;
            }
            var fileData = (byte[])entities[0].GetColumnValue("StFileData");
            return fileData;
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
        public string WriteBusinessTransaction(string contractId, List<ServiceData> services)
        {
            string documentId = null;

            SOMRequestInput<WriteBusinessTransactionInput> somRequestInput = new SOMRequestInput<WriteBusinessTransactionInput>
            {
                InputArguments = new WriteBusinessTransactionInput
                {
                    ContractId = contractId,
                    Services = services
                }
            };

            using (var client = new SOMClient())
            {
                documentId = client.Post<SOMRequestInput<WriteBusinessTransactionInput>, string>("api/SOM.ST/Billing/WriteBusinessTransaction", somRequestInput);
            }

            return documentId;
        }

        public void PostCallDetailsToOM(Guid requestId)
        {

        }

        #endregion

        #region Private
        private bool ByteArrayToFile(string fileName, byte[] byteArray)
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
        private void DefineStyles()
        {
            MigraDoc.DocumentObjectModel.Style style = this.document.Styles["Normal"];
            style.Font.Name = "Verdana";

            style = this.document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = this.document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            style = this.document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;

            style = this.document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }
        private void CreatePages(CallDetailRequest request, int numberOfRecords)
        {
            Section section = this.document.AddSection();
            Paragraph paragraph = section.Footers.Primary.AddParagraph();

            this.addressFrame = section.AddTextFrame();
            this.addressFrame.Height = "10.0cm";
            this.addressFrame.Width = "20.0cm";
            this.addressFrame.Left = ShapePosition.Left;
            this.addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            this.addressFrame.Top = "3.0cm";
            this.addressFrame.RelativeVertical = RelativeVertical.Page;

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "5cm";
            paragraph.Style = "Reference";
            paragraph.AddFormattedText("Call Details", TextFormat.Bold);

            Paragraph paragraph2 = this.addressFrame.AddParagraph();
            if (request.Name != null)
            {
                paragraph2.AddText(request.Name);
                paragraph2.AddLineBreak();
            }
            if (request.PhoneNumber != null)
            {
                paragraph2.AddText(request.PhoneNumber);
                paragraph2.AddLineBreak();
            }
            if (request.UserName != null)
            {
                paragraph2.AddText(request.UserName);
                paragraph2.AddLineBreak();
            }
            if (request.CSO != null)
            {
                paragraph2.AddText(request.CSO);
                paragraph2.AddLineBreak();
            }
            if (request.CSO != null)
            {
                paragraph2.AddText(request.Date);
            }

            bool isFirtsPage = true;
            for (int i = 0; i < request.Items.Count; i += numberOfRecords)
            {
                List<CallDetails> smallList = new List<CallDetails>();
                smallList = request.Items.Skip(i).Take(numberOfRecords).ToList();
                DrawPageRecords(smallList, isFirtsPage);
                isFirtsPage = false;
            }
        }
        private void DrawPageRecords(List<CallDetails> items, bool isFirstPage)
        {
            // Create the item table
            MigraDoc.DocumentObjectModel.Tables.Table table1 = new MigraDoc.DocumentObjectModel.Tables.Table();
            if (isFirstPage)
            {
                table1 = this.document.LastSection.AddTable();
            }
            else
            {
                Section section = document.AddSection();
                table1 = this.document.LastSection.AddTable();
            }
            table1.Style = "Table";
            table1.Borders.Width = 0.25;
            table1.Borders.Left.Width = 0.5;
            table1.Borders.Right.Width = 0.5;
            table1.Rows.LeftIndent = 0;

            MigraDoc.DocumentObjectModel.Tables.Column column = table1.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table1.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table1.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table1.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table1.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;


            Row row = table1.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("Date");
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;

            row.Cells[1].AddParagraph("Duration (min)");
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;

            row.Cells[2].AddParagraph("Calling Number");
            row.Cells[2].Format.Font.Bold = false;
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[2].VerticalAlignment = VerticalAlignment.Bottom;

            row.Cells[3].AddParagraph("Called Number");
            row.Cells[3].Format.Font.Bold = false;
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[3].VerticalAlignment = VerticalAlignment.Bottom;

            row.Cells[4].AddParagraph("Amount (SYR)");
            row.Cells[4].Format.Font.Bold = false;
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[4].VerticalAlignment = VerticalAlignment.Bottom;

            table1.SetEdge(0, 0, 5, 1, Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 0.75, Color.Empty);


            Paragraph paragraph3 = this.document.LastSection.AddParagraph();
            foreach (CallDetails item in items)
            {

                Row row1 = new Row();
                row1 = table1.AddRow();

                row1.TopPadding = 1.5;

                row1.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[1].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[2].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[3].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[4].VerticalAlignment = VerticalAlignment.Center;
                //row1.Cells[5].VerticalAlignment = VerticalAlignment.Center;

                paragraph3 = row1.Cells[0].AddParagraph();
                paragraph3.AddText(item.CallDate.ToString());

                paragraph3 = row1.Cells[1].AddParagraph();
                paragraph3.AddText(item.Duration.ToString());

                paragraph3 = row1.Cells[2].AddParagraph();
                paragraph3.AddText(item.CallingNumber.ToString());

                paragraph3 = row1.Cells[3].AddParagraph();
                paragraph3.AddText(item.CalledNumber.ToString());

                paragraph3 = row1.Cells[4].AddParagraph();
                paragraph3.AddText(item.Amount.ToString());


                table1.SetEdge(0, table1.Rows.Count - 1, 5, 1, Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.Single, 0.75);
            }
        }
        private void FillContent(CallDetailRequest request)
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
            paragraph.AddText(request.Date);

            foreach (CallDetails item in request.Items)
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

                //row1.Cells[5].VerticalAlignment = VerticalAlignment.Center;
                //row1.Cells[5].MergeDown = 1;


                paragraph = row1.Cells[0].AddParagraph();
                paragraph.AddText(item.CallDate.ToString());

                paragraph = row1.Cells[1].AddParagraph();
                paragraph.AddText(item.Duration.ToString());

                paragraph = row1.Cells[2].AddParagraph();
                paragraph.AddText(item.CallingNumber.ToString());

                paragraph = row1.Cells[3].AddParagraph();
                paragraph.AddText(item.CalledNumber.ToString());

                paragraph = row1.Cells[4].AddParagraph();
                paragraph.AddText(item.Amount.ToString());

                //paragraph = row1.Cells[5].AddParagraph();
                //paragraph.AddText(item.DurationUnit.ToString());

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
        #endregion

        #region Mappers
        public CallDetailsEntity CallDetailsToEntityMapper(CallDetails callDetails)
        {
            return new CallDetailsEntity()
            {
                Amount = callDetails.Amount,
                Date = callDetails.CallDate,
                Duration = callDetails.Duration / 60,
                Number = callDetails.CalledNumber,
                Place = callDetails.CallingNumber //To check Place
            };
        }
        #endregion

    }
}
