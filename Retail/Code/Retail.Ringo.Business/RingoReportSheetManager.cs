using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Aspose.Cells;
using Retail.Ringo.Entities;
using Vanrise.Entities;
using ICSharpCode.SharpZipLib.Zip;
using Vanrise.Common;

namespace Retail.Ringo.Business
{
    public class RingoReportSheetManager
    {
        public Stream GenerateMNPReport(RingoReportFilter filter)
        {
            Vanrise.Common.Utilities.ActivateAspose();
            RingoMessageManager ringoMessageManager = new RingoMessageManager();

            string physicalFilePath = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["MNPReportPath"]);

            Workbook wbk = new Workbook(physicalFilePath);

            UpdateCellValue(wbk, 0, 3, 1, ringoMessageManager.GetRingoMessageTotal(new RingoMessageFilter
             {
                 From = filter.FromDate.AddMonths(-1),
                 To = filter.ToDate.AddMonths(-1),
                 Sender = "ICSI",
                 MessageTypes = new List<int> { 1 }
             }));
            UpdateCellValue(wbk, 0, 6, 1, ringoMessageManager.GetRingoMessageTotal(
                new RingoMessageFilter
             {
                 From = filter.FromDate,
                 To = filter.ToDate,
                 Sender = "ICSI",
                 MessageTypes = new List<int> { 1 }
             }));
            //1r
            UpdateWorkSheetByColumn(ringoMessageManager.GetSenderRingoMessageRecords(new RingoMessageFilter
            {
                MessageTypes = new List<int>() { 1 },
                Recipient = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 2, 13, 1, 170, 168);
            //2r
            UpdateWorkSheetByColumn(ringoMessageManager.GetRingoMessageCountEntityBySender_LastDay(new RingoMessageFilter
            {
                MessageTypes = new List<int>() { 1 },
                Recipient = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, sheetIndex: 3, rowIndex: 13, columnIndex: 1, valueColumn: 170, count: 168);

            //3r
            UpdateWorkSheetByRow(ringoMessageManager.GetSenderRingoMessageRecords(new RingoMessageFilter
            {
                StateRequests = new List<int>() { 9 },
                Recipient = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 4, 11, 3, 12, 168);

            UpdateWorkSheetByRow(ringoMessageManager.GetSenderRingoMessageRecords(new RingoMessageFilter
            {
                StateRequests = new List<int>() { 8 },
                Recipient = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 4, 11, 3, 14, 168);

            UpdateWorkSheetByRow(ringoMessageManager.GetSenderRingoMessageRecords(new RingoMessageFilter
            {
                StateRequests = new List<int>() { 7 },
                Recipient = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 4, 11, 3, 15, 168);

            UpdateWorkSheetByRow(ringoMessageManager.GetSenderRingoMessageRecords(new RingoMessageFilter
            {
                StateRequests = new List<int>() { 1 },
                Recipient = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 4, 11, 3, 16, 168);

            //3d
            UpdateWorkSheetByRow(ringoMessageManager.GetRecipientRingoMessageRecords(new RingoMessageFilter
            {
                StateRequests = new List<int>() { 9 },
                Sender = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 5, 11, 3, 12, 168);

            UpdateWorkSheetByRow(ringoMessageManager.GetRecipientRingoMessageRecords(new RingoMessageFilter
            {
                StateRequests = new List<int>() { 8 },
                Sender = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 5, 11, 3, 14, 168);

            UpdateWorkSheetByRow(ringoMessageManager.GetRecipientRingoMessageRecords(new RingoMessageFilter
            {
                StateRequests = new List<int>() { 7 },
                Sender = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 5, 11, 3, 15, 168);

            UpdateWorkSheetByRow(ringoMessageManager.GetRecipientRingoMessageRecords(new RingoMessageFilter
            {
                StateRequests = new List<int>() { 1 },
                Sender = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 5, 11, 3, 16, 168);

            //5t
            UpdateWorkSheetByColumn(ringoMessageManager.GetSenderRingoMessageRecords_CTE(new RingoMessageFilter
            {
                MessageTypes = new List<int>() { 6 },
                Recipient = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 8, 13, 1, 170, 168);

            //5s
            UpdateWorkSheetByColumn(ringoMessageManager.GetSenderRingoMessageRecords_CTE(new RingoMessageFilter
            {
                MessageTypes = new List<int>() { 6 },
                Recipient = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 9, 13, 1, 170, 168);


            //7N
            UpdateWorkSheetByColumn(ringoMessageManager.GetSenderRingoMessageRecords_CTE(new RingoMessageFilter
            {
                MessageTypes = new List<int> { 6 },
                Recipient = "ICSI",
                From = DateTime.Parse("2015-05-01"),
                To = filter.ToDate
            }), wbk, 12, 13, 1, 170, 168);

            //8r 13
            UpdateWorkSheetByRow(ringoMessageManager.GetSenderRingoMessageRecords_EightSheet(new RingoMessageFilter
            {
                MessageTypes = new List<int> { 1 },
                Recipient = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            },
            new RingoMessageFilter
            {
                MessageTypes = new List<int> { 6 },
                Recipient = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 13, 11, 3, 12, 168);

            //8r 14
            UpdateWorkSheetByRow(ringoMessageManager.GetSenderRingoMessageRecords_EightSheet(new RingoMessageFilter
            {
                MessageTypes = new List<int> { 2 },
                Recipient = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            },
            new RingoMessageFilter
            {
                MessageTypes = new List<int> { 6 },
                Recipient = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 13, 11, 3, 13, 168);

            //8r 16
            UpdateWorkSheetByRow(ringoMessageManager.GetSenderRingoMessageRecords(new RingoMessageFilter
            {
                MessageTypes = new List<int> { 6 },
                Recipient = "ICSI",
                From = filter.FromDate,
                To = filter.ToDate
            }), wbk, 13, 11, 3, 15, 168);

            return wbk.SaveToStream();
        }

        public byte[] GenerateTCRReport(TCRRingoReportFilter filter)
        {
            Dictionary<string, byte[]> dicAllData = new Dictionary<string, byte[]>();
            RingoMessageManager ringoMessageManager = new RingoMessageManager();


            switch (filter.ReportType)
            {
                case TCRReportType.Sintesi:
                    filter.From = new DateTime(filter.From.Value.Year, filter.From.Value.Month, 01, 00, 00, 00);
                    filter.To = new DateTime(filter.From.Value.Year, filter.From.Value.Month, DateTime.DaysInMonth(filter.From.Value.Year, filter.From.Value.Month), 23, 59, 59);
                    GenerateSintesiReport(filter, dicAllData, ringoMessageManager);
                    break;
                case TCRReportType.Dettaglio:
                    filter.From = new DateTime(filter.From.Value.Year, filter.From.Value.Month, filter.From.Value.Day, 00, 00, 00);
                    filter.To = new DateTime(filter.From.Value.Year, filter.From.Value.Month, filter.From.Value.Day, 23, 59, 59);
                    GenerateDettaglioReport(filter, dicAllData, ringoMessageManager);
                    break;
            }

            return Compress(dicAllData);
        }

        #region Private Methods
        void GenerateDettaglioReport(TCRRingoReportFilter filter, Dictionary<string, byte[]> dicAllData, RingoMessageManager ringoMessageManager)
        {
            IEnumerable<DettaglioRingoMessageEntity> listDettaglioRecipient = ringoMessageManager.GetDettaglioRingoMessageEntityByRecipient(new TCRRingoReportFilter
            {
                Operator = filter.Operator,
                From = filter.From,
                To = filter.To
            });

            IEnumerable<DettaglioRingoMessageEntity> listDettaglioSender = ringoMessageManager.GetDettaglioRingoMessageEntityBySender(new TCRRingoReportFilter
            {
                Operator = filter.Operator,
                From = filter.From,
                To = filter.To
            });

            foreach (var item in GetData(listDettaglioRecipient, filter, true))
            {
                if (!dicAllData.ContainsKey(item.Key))
                    dicAllData.Add(item.Key, item.Value);
            }

            foreach (var item in GetData(listDettaglioSender, filter, false))
            {
                if (!dicAllData.ContainsKey(item.Key))
                    dicAllData.Add(item.Key, item.Value);
            }
        }

        void GenerateSintesiReport(TCRRingoReportFilter filter, Dictionary<string, byte[]> dicAllData, RingoMessageManager ringoMessageManager)
        {
            IEnumerable<SintesiRingoMessageEntity> listSintesiRecipient = ringoMessageManager.GetSintesiRingoMessageEntityByRecipient(new TCRRingoReportFilter
            {
                Operator = filter.Operator,
                From = filter.From,
                To = filter.To
            });

            IEnumerable<SintesiRingoMessageEntity> listSintesiSender = ringoMessageManager.GetSintesiRingoMessageEntityBySender(new TCRRingoReportFilter
            {
                Operator = filter.Operator,
                From = filter.From,
                To = filter.To
            });

            foreach (var item in GetData(listSintesiRecipient, filter, true))
            {
                if (!dicAllData.ContainsKey(item.Key))
                    dicAllData.Add(item.Key, item.Value);
            }

            foreach (var item in GetData(listSintesiSender, filter, false))
            {
                if (!dicAllData.ContainsKey(item.Key))
                    dicAllData.Add(item.Key, item.Value);
            }
        }

        private Dictionary<string, byte[]> GetData(IEnumerable<SintesiRingoMessageEntity> listSintesiRecipient, TCRRingoReportFilter filter, bool recipient)
        {
            List<SintesiRingoMessageEntity> listOperators = listSintesiRecipient.ToList();
            Dictionary<string, byte[]> dicData = new Dictionary<string, byte[]>();

            Dictionary<string, List<SintesiRingoMessageEntity>> groupedOperatorsData = new Dictionary<string, List<SintesiRingoMessageEntity>>();

            foreach (var item in listSintesiRecipient)
            {
                var list = groupedOperatorsData.GetOrCreateItem(item.Operator, () => new List<SintesiRingoMessageEntity>());
                list.Add(item);
            }

            int incremental = 0;
            int transactionCount = 0;
            int sumTransferredCredit = 0;
            foreach (var operatorData in groupedOperatorsData)
            {
                var sb = new StringBuilder();
                incremental = 1;
                transactionCount = 0;
                sumTransferredCredit = 0;

                if (recipient)
                    sb.AppendLine(SintesiRingoOperatorRecipientHeader(operatorData.Value.First(), filter));
                else
                    sb.AppendLine(SintesiRingoOperatorSenderHeader(operatorData.Value.First(), filter));

                var data = operatorData.Value;

                foreach (var item in data)
                {
                    sb.AppendLine(SintesiRingoOperatorBody(item, incremental));
                    sumTransferredCredit += item.TotalTransferredCredit;
                    transactionCount += item.NumberOfRows;
                    incremental++;
                }
                if (recipient)
                    sb.AppendLine(SintesiRingoOperatorRecipientFooter(data.First(), filter,
                        transactionCount, sumTransferredCredit, incremental + 2));
                else
                    sb.AppendLine(SintesiRingoOperatorSenderFooter(data.First(), filter,
                       transactionCount, sumTransferredCredit, incremental + 2));

                string fileName;
                if (recipient)
                    fileName = "TCR_Ringo_" + data.First().Operator + "_" + filter.From.Value.ToString("yyyyMM");
                else
                    fileName = "TCR_" + data.First().Operator + "_Ringo_" + filter.From.Value.ToString("yyyyMM");

                dicData.Add(fileName, System.Text.Encoding.UTF8.GetBytes(sb.ToString()));
            }


            return dicData;
        }

        private Dictionary<string, byte[]> GetData(IEnumerable<DettaglioRingoMessageEntity> listDettaglioRecipient, TCRRingoReportFilter filter, bool recipient)
        {
            List<DettaglioRingoMessageEntity> listOperators = listDettaglioRecipient.ToList();
            Dictionary<string, byte[]> dicData = new Dictionary<string, byte[]>();

            bool newOperator = true;
            var sb = new StringBuilder();

            int incremental = 1;
            int transactionCount = 0;
            int sumTransferredCredit = 0;

            for (int i = 0; i < listOperators.Count(); i++)
            {
                if (newOperator)
                {
                    incremental = 1;
                    transactionCount = 0;
                    sumTransferredCredit = 0;

                    newOperator = false;
                    if (recipient)
                        sb.AppendLine(DettaglioRingoOperatorRecipientHeader(listOperators[i], filter));
                    else
                        sb.AppendLine(DettaglioRingoOperatorSenderHeader(listOperators[i], filter));
                }

                sumTransferredCredit = sumTransferredCredit + listOperators[i].TransferredCredit;
                transactionCount = transactionCount + 1;

                if (i + 1 < listOperators.Count())
                {
                    if (listOperators[i].Operator != listOperators[i + 1].Operator)
                    {
                        sb.AppendLine(DettaglioRingoOperatorBody(listOperators[i], incremental));

                        if (recipient)
                            sb.AppendLine(DettaglioRingoOperatorRecipientFooter(listOperators[i], filter,
                                transactionCount, sumTransferredCredit, incremental + 2));
                        else
                            sb.AppendLine(DettaglioRingoOperatorSenderFooter(listOperators[i], filter,
                                transactionCount, sumTransferredCredit, incremental + 2));

                        var myString = sb.ToString();
                        var myByteArray = System.Text.Encoding.UTF8.GetBytes(myString);
                        string fileName;
                        if (recipient)
                            fileName = "Ringo_" + listOperators[i].Operator + "_" + filter.From.Value.ToString("yyyyMMdd");
                        else
                            fileName = listOperators[i].Operator + "_Ringo_" + filter.From.Value.ToString("yyyyMMdd");

                        dicData.Add(fileName, myByteArray);
                        newOperator = true;
                    }
                    else
                    {
                        sb.AppendLine(DettaglioRingoOperatorBody(listOperators[i], incremental));
                    }
                }
                else
                {
                    sb.AppendLine(DettaglioRingoOperatorBody(listOperators[i], incremental));

                    if (recipient)
                        sb.AppendLine(DettaglioRingoOperatorRecipientFooter(listOperators[i], filter, transactionCount, sumTransferredCredit, incremental + 2));
                    else
                        sb.AppendLine(DettaglioRingoOperatorSenderFooter(listOperators[i], filter, transactionCount, sumTransferredCredit, incremental + 2));

                    var myString = sb.ToString();
                    var myByteArray = System.Text.Encoding.UTF8.GetBytes(myString);

                    string fileName;
                    if (recipient)
                        fileName = "Ringo_" + listOperators[i].Operator + "_" + filter.From.Value.ToString("yyyyMMdd");
                    else
                        fileName = listOperators[i].Operator + "_Ringo_" + filter.From.Value.ToString("yyyyMMdd");

                    dicData.Add(fileName, myByteArray);
                }

                incremental++;
            }
            return dicData;
        }

        private static byte[] Compress(Dictionary<string, byte[]> data)
        {
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            ZipOutputStream s = new ZipOutputStream(memoryStream);
            s.SetLevel(9);
            foreach (var item in data)
            {
                ZipEntry entry = new ZipEntry(item.Key + ".txt");
                s.PutNextEntry(entry);
                s.Write(item.Value, 0, item.Value.Length);
            }
            s.Finish();
            s.Close();
            return memoryStream.ToArray();
        }

        #region Sintesi Functions
        string SintesiRingoOperatorRecipientHeader(SintesiRingoMessageEntity sintesiRingoMessageEntity, TCRRingoReportFilter filter)
        {
            return (string.Format(" RS{0}NOVA{1}ICSI{2}RINGOT{3}", sintesiRingoMessageEntity.Network,
            sintesiRingoMessageEntity.Network == sintesiRingoMessageEntity.Operator ? "    " : sintesiRingoMessageEntity.Operator,
            filter.From.Value.ToString("yyyyMM"), filter.From.Value.ToString("yyMM")));
        }

        string SintesiRingoOperatorBody(SintesiRingoMessageEntity sintesiRingoMessageEntity, int incremental)
        {
            return (string.Format(" 20{0}{1}{2}{3}",
                incremental.ToString("D10"), sintesiRingoMessageEntity.MessageDate.ToString("yyyyMMdd"), sintesiRingoMessageEntity.TotalTransferredCredit.ToString("D10"),
                sintesiRingoMessageEntity.NumberOfRows.ToString("D10")));
        }

        string SintesiRingoOperatorRecipientFooter(SintesiRingoMessageEntity sintesiRingoMessageEntity,
            TCRRingoReportFilter filter, int transactionCount, int sumTransferredCredit, int incremental)
        {
            return (string.Format(" RF{0}NOVA{1}ICSI{2}RINGOT{3}{4}{5}{6}", sintesiRingoMessageEntity.Operator,
                sintesiRingoMessageEntity.Network == sintesiRingoMessageEntity.Operator ? "    " : sintesiRingoMessageEntity.Network,
                filter.From.Value.ToString("yyyyMM"), filter.From.Value.ToString("yyMM"), transactionCount.ToString("D10"), sumTransferredCredit.ToString("D10"),
                incremental.ToString("D10")));
        }

        string SintesiRingoOperatorSenderHeader(SintesiRingoMessageEntity sintesiRingoMessageEntity,
            TCRRingoReportFilter filter)
        {
            return (string.Format(" RSNOVA{0}ICSI{1}{2}RINGIN{3}", sintesiRingoMessageEntity.Network,
            sintesiRingoMessageEntity.Network == sintesiRingoMessageEntity.Operator ? "    " : sintesiRingoMessageEntity.Operator,
            filter.From.Value.ToString("yyyyMM"), filter.From.Value.ToString("yyMM")));
        }

        string SintesiRingoOperatorSenderFooter(SintesiRingoMessageEntity sintesiRingoMessageEntity,
            TCRRingoReportFilter filter, int transactionCount, int sumTransferredCredit, int incremental)
        {
            return (string.Format(" RFNOVA{0}ICSI{1}{2}RINGIN{3}{4}{5}{6}", sintesiRingoMessageEntity.Network,
                sintesiRingoMessageEntity.Network == sintesiRingoMessageEntity.Operator ? "    " : sintesiRingoMessageEntity.Operator,
                filter.From.Value.ToString("yyyyMM"), filter.From.Value.ToString("yyMM"), transactionCount.ToString("D10"), sumTransferredCredit.ToString("D10"),
                incremental.ToString("D10")));
        }

        #endregion


        #region Dettaglio Functions
        string DettaglioRingoOperatorRecipientHeader(DettaglioRingoMessageEntity dettaglioRingoMessageEntity,
            TCRRingoReportFilter filter)
        {
            return (string.Format(" PC{0}NOVA{1}ICSI{2}RINGOT{3}", dettaglioRingoMessageEntity.Operator,
            dettaglioRingoMessageEntity.Network == dettaglioRingoMessageEntity.Operator ? "    " : dettaglioRingoMessageEntity.Network,
            filter.From.Value.ToString("yyyyMM"), filter.From.Value.ToString("yyMM")));
        }

        string DettaglioRingoOperatorBody(DettaglioRingoMessageEntity dettaglioRingoMessageEntity, int incremental)
        {
            return (string.Format(" 10{0}{1}{2}",
                incremental.ToString("D10"), dettaglioRingoMessageEntity.RecipientRequestCode, dettaglioRingoMessageEntity.TransferredCredit.ToString("D10")));
        }

        string DettaglioRingoOperatorRecipientFooter(DettaglioRingoMessageEntity dettaglioRingoMessageEntity,
            TCRRingoReportFilter filter, int transactionCount, int sumTransferredCredit, int incremental)
        {
            return (string.Format(" EF{0}NOVA{1}ICSI{2}RINGOT{3}{4}{5}{6}", dettaglioRingoMessageEntity.Network,
                dettaglioRingoMessageEntity.Network == dettaglioRingoMessageEntity.Operator ? "    " : dettaglioRingoMessageEntity.Operator,
                filter.From.Value.ToString("yyyyMM"), filter.From.Value.ToString("yyMM"), transactionCount.ToString("D10"), sumTransferredCredit.ToString("D10"),
                incremental.ToString("D10")));
        }

        string DettaglioRingoOperatorSenderHeader(DettaglioRingoMessageEntity dettaglioRingoMessageEntity, TCRRingoReportFilter filter)
        {
            return (string.Format(" PCNOVA{0}ICSI{1}{2}RINGIN{3}", dettaglioRingoMessageEntity.Network,
            dettaglioRingoMessageEntity.Network == dettaglioRingoMessageEntity.Operator ? "    " : dettaglioRingoMessageEntity.Operator,
            filter.From.Value.ToString("yyyyMM"), filter.From.Value.ToString("yyMM")));
        }

        string DettaglioRingoOperatorSenderFooter(DettaglioRingoMessageEntity dettaglioRingoMessageEntity,
            TCRRingoReportFilter filter, int transactionCount, int sumTransferredCredit, int incremental)
        {
            return (string.Format(" EFNOVA{0}ICSI{1}{2}RINGIN{3}{4}{5}{6}", dettaglioRingoMessageEntity.Network,
                dettaglioRingoMessageEntity.Network == dettaglioRingoMessageEntity.Operator ? "    " : dettaglioRingoMessageEntity.Operator,
                filter.From.Value.ToString("yyyyMM"), filter.From.Value.ToString("yyMM"), transactionCount.ToString("D10"), sumTransferredCredit.ToString("D10"),
                incremental.ToString("D10")));
        }

        #endregion
        void UpdateWorkSheetByRow(Dictionary<string, RingoMessageCountEntity> data, Workbook wbk, int sheetIndex, int rowIndex, int columnIndex, int valueRow, int count)
        {

            for (int i = columnIndex; i < count + columnIndex; i++)
            {
                var cellValue = wbk.Worksheets[sheetIndex].Cells[rowIndex, i].Value as string;
                RingoMessageCountEntity entity;
                if (data.TryGetValue(cellValue, out entity))
                    wbk.Worksheets[sheetIndex].Cells[valueRow, i].Value = entity.Value;
            }
        }
        void UpdateWorkSheetByColumn(Dictionary<string, RingoMessageCountEntity> data, Workbook wbk, int sheetIndex, int rowIndex, int columnIndex, int valueColumn, int count)
        {
            for (int i = rowIndex; i < count + rowIndex; i++)
            {
                var cellValue = wbk.Worksheets[sheetIndex].Cells[i, columnIndex].Value as string;
                RingoMessageCountEntity entity;
                if (data.TryGetValue(cellValue as string, out entity))
                    wbk.Worksheets[sheetIndex].Cells[i, valueColumn].Value = entity.Value;
            }
        }
        void UpdateCellValue(Workbook wbk, int sheet, int row, int column, object value)
        {
            wbk.Worksheets[sheet].Cells[row, column].Value = value;
        }

        #endregion
    }
}
