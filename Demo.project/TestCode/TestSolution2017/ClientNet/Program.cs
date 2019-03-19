using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Interceptor;
using System.IO;
using System.Data.Entity.Core.EntityClient;

namespace ClientNet
{
    class Program
    {
        static void Main(string[] args)
        {

            ParseSQLPostScripts();
            Tuple<string, decimal> tup = new Tuple<string, decimal>("Service Name", 35432);
            string serialized = Vanrise.Common.Serializer.Serialize(tup);
            //StandardOnlyLib.VRTCPCommunication tcpComm = new StandardOnlyLib.VRTCPCommunication();
            //tcpComm.ConnectToServer();

            //Castle.DynamicProxy.ProxyGenerator proxyGenerator = new Castle.DynamicProxy.ProxyGenerator();
            //var testProxy = proxyGenerator.CreateInterfaceProxyWithoutTarget<ITestProxy>(new TestProxyInterceptor());
            //string returnedValue1= testProxy.DoSomething("call 1");
            //Console.WriteLine(returnedValue1);
            //testProxy.DoSomething("call 2");
            string[] allLines = File.ReadAllLines(@"C:\Test\ServiceURLs.txt");
            string serviceURL = allLines[allLines.Length - 1];
            Console.WriteLine("CLIENT: Service URL is: {0}", serviceURL);

            //           Console.WriteLine("Enter Number of Lines...");
            //           int nbOfLines = int.Parse(Console.ReadLine());
            //           StringBuilder builder = new StringBuilder();
            //           for(int i =0;i< nbOfLines; i++)
            //           {
            //               builder.AppendLine("gfdsgfdsgfdgfdg sgsgfd");
            //           }
            //string response = null;

            //           try
            //           {
            //              Vanrise.Common.VRInterAppCommunication.CreateServiceClient<ServerNet.ITestService>(serviceURL, (client) =>
            //               {
            //                   response = client.PostInput(new ServerNet.ServiceInput { StringProp = builder.ToString() });
            //               });
            //           }
            //           catch(Exception ex)
            //           {
            //               Console.WriteLine(ex.Message);
            //           } 
            //response = null;
            

            var isConnected1 = Vanrise.Common.VRInterAppCommunication.TryCreateServiceClient<ServerNet.ITestService2>(allLines[allLines.Length - 2], (client) =>
            { });

            var isConnected2 = Vanrise.Common.VRInterAppCommunication.TryCreateServiceClient<ServerNet.ITestService>(allLines[allLines.Length - 2], (client) =>
            { });

            var isConnected3 = Vanrise.Common.VRInterAppCommunication.TryCreateServiceClient<ServerNet.ITestService2>(serviceURL, (client) =>
            { });

            var isConnected4 = Vanrise.Common.VRInterAppCommunication.TryCreateServiceClient<ServerNet.ITestService2>(serviceURL + "anotherService", (client) =>
            { });

           
            var isConnected = Vanrise.Common.VRInterAppCommunication.TryCreateServiceClient<ServerNet.ITestService>(serviceURL, (client) =>
            {
                client.DoSomething("value1");
                Console.WriteLine($"2 * 4 is {client.Multiply(2, 4)}");
                //Console.WriteLine($"34 * 145 is {client.Multiply(34, 145)}");
                Console.WriteLine($"34 * 45 is {client.Multiply(34, 45)}");
            });
            Vanrise.Common.VRInterAppCommunication.CreateServiceClient<ServerNet.ITestService>(serviceURL, (client) =>
            {
                client.DoSomething("value1");
                Console.WriteLine($"2 * 4 is {client.Multiply(2, 4)}");
                //Console.WriteLine($"34 * 145 is {client.Multiply(34, 145)}");
                Console.WriteLine($"34 * 45 is {client.Multiply(34, 45)}");
            });
            Vanrise.Common.VRInterAppCommunication.TryCreateServiceClient<ServerNet.ITestService>(serviceURL, (client) =>
            {
                client.DoSomething("value1");
                Console.WriteLine($"2 * 4 is {client.Multiply(2, 4)}");
                //Console.WriteLine($"34 * 145 is {client.Multiply(34, 145)}");
                Console.WriteLine($"34 * 45 is {client.Multiply(34, 45)}");
            });
            Vanrise.Common.VRInterAppCommunication.CreateServiceClient<ServerNet.ITestService>(serviceURL, (client) =>
            {
                client.DoSomething("value1");
                Console.WriteLine($"2 * 4 is {client.Multiply(2, 4)}");
                //Console.WriteLine($"34 * 145 is {client.Multiply(34, 145)}");
                Console.WriteLine($"34 * 45 is {client.Multiply(34, 45)}");
            });
            Console.ReadKey();
        }

        private static void ParseSQLPostScripts()
        {
            StringBuilder verifiedOutputBuilder = new StringBuilder();
            string scriptContent = System.IO.File.ReadAllText(@"C:\TFS\TOneV2\Code\TOneV2\SQL.TOneConfiguration\Security.PostDeployment.sql");
            string regExPattern = @"with cte_data(.*?)values\(s\.(.*?)\)\;";// @"with cte_data(.*?)when not matched by target then";//"with cte_data((.|\n)*)values";
            var matches = System.Text.RegularExpressions.Regex.Matches(scriptContent, regExPattern, System.Text.RegularExpressions.RegexOptions.Singleline);
            int count = matches.Count;
            foreach (System.Text.RegularExpressions.Match m in matches)
            {
                string mergeExp = m.Value;
                var colsMatch = System.Text.RegularExpressions.Regex.Match(mergeExp, @"\((.*?)\)");
                string[] columnNames = colsMatch.Value.Replace("(", "").Replace(")", "").Split(',');
                
                StringBuilder dataPatternBuilder = new StringBuilder(@"\(");
                for (int i = 0; i < columnNames.Length; i++)
                {
                    if (i > 0)
                        dataPatternBuilder.Append(",");
                    dataPatternBuilder.Append("(.*?)");
                }
                dataPatternBuilder.Append(@"\)");

                var dataMatches = System.Text.RegularExpressions.Regex.Matches(mergeExp, dataPatternBuilder.ToString(), System.Text.RegularExpressions.RegexOptions.Singleline);

                string mergeExpWithoutData = mergeExp.Substring(mergeExp.LastIndexOf("merge	"));
                var tableNameMatch = System.Text.RegularExpressions.Regex.Match(mergeExpWithoutData, @"merge	(.*?) as t");
                var tableNameParts = tableNameMatch.Value.Replace("merge	", "").Replace(" as t", "").Split('.');
                string schemaName = tableNameParts[0];
                string tableName = tableNameParts[1];

                var identifierColumnsMatch = System.Text.RegularExpressions.Regex.Match(mergeExpWithoutData, @"using	cte_data as s(.*?)when matched then", System.Text.RegularExpressions.RegexOptions.Singleline);
                var identifierColumns = identifierColumnsMatch.Value.Replace("using	cte_data as s", "").Replace("when matched then", "").Replace("on		", "").Replace("1=1", "").Replace("and", "~").Split('~').Select(itm => itm.Split('=')[0].Replace("t.", "").Trim()).Where(itm => !string.IsNullOrWhiteSpace(itm)).ToList();

                var columnsToInsertMatch = System.Text.RegularExpressions.Regex.Match(mergeExpWithoutData, @"when not matched by target then(.*?)\)", System.Text.RegularExpressions.RegexOptions.Singleline);
                var columnsToInsert = columnsToInsertMatch.Value.Replace("when not matched by target then", "").Replace(")", "").Replace("insert(", "").Split(',').Select(itm => itm.Trim()).ToList();

                var columnsToUpdateMatch = System.Text.RegularExpressions.Regex.Match(mergeExpWithoutData, @"update set(.*?)when not matched by target then", System.Text.RegularExpressions.RegexOptions.Singleline);
                List<string> columnsToUpdate;
                if (columnsToUpdateMatch.Value.Contains("--"))
                    columnsToUpdate = null;
                else
                    columnsToUpdate = columnsToUpdateMatch.Value.Replace("update set", "").Replace("when not matched by target then", "").Split(',').Select(itm => itm.Split('=')[0].Trim()).ToList();

                List<Dictionary<string, Object>> dataScript = ExtractDataFromMergeScript(mergeExp, columnNames);

                string mergeOutput = GenerateMergeScriptToVerify(schemaName, tableName, columnNames, identifierColumns, columnsToInsert, columnsToUpdate, dataScript);
                verifiedOutputBuilder.Append(mergeOutput);
                verifiedOutputBuilder.AppendLine();
                verifiedOutputBuilder.AppendLine();
            }
            string verifiedOutput = verifiedOutputBuilder.ToString();
        }

        private static List<Dictionary<string, object>> ExtractDataFromMergeScript(string mergeExp, string[] columnNames)
        {
            List<Dictionary<string, object>> dataScripts = new List<Dictionary<string, object>>();
            int startingOfDataIndex = mergeExp.IndexOf("as (select * from (values") + 25;
            int endOfDataIndex = mergeExp.LastIndexOf("merge	");
            string dataSection = mergeExp.Substring(startingOfDataIndex, endOfDataIndex - startingOfDataIndex);

            Dictionary<string, object> currentRow = null;
            string currentColumnName = null;
            StringBuilder currentFieldBuilder = null;
            int characterIndex = 0;
            bool isStringField = false;
            bool readyForNewField = false;
            foreach (Char character in dataSection)
            {
                if (currentFieldBuilder != null)
                {
                    if (isStringField)
                    {
                        if (character != '\'')
                        {
                            currentFieldBuilder.Append(character);
                        }
                        else
                        {
                            if (dataSection.Length > characterIndex + 1 && dataSection[characterIndex + 1] == '\'')//next value is '
                            {
                                currentFieldBuilder.Append(character);
                                continue;
                            }
                            if (dataSection[characterIndex - 1] == '\'')//previous value is '
                            {
                                currentFieldBuilder.Append(character);
                                continue;
                            }
                            AddFieldToRow(currentRow, ref currentColumnName, ref currentFieldBuilder, isStringField, ref readyForNewField);
                        }
                    }
                    else
                    {
                        if(character != ',' && character != ')')
                        {
                            currentFieldBuilder.Append(character);
                        }
                        else
                        {
                            AddFieldToRow(currentRow, ref currentColumnName, ref currentFieldBuilder, isStringField, ref readyForNewField);
                            if (character == ')')
                                currentRow = null;
                            else if (character == ',')
                                readyForNewField = true;
                        }
                    }
                }
                else
                {
                    switch(character)
                    {
                        case '(':
                            {
                                currentRow = new Dictionary<string, object>();
                                readyForNewField = true;
                                dataScripts.Add(currentRow);
                            }
                            break;
                        case ')':
                            {
                                if (currentRow.Count != columnNames.Length)
                                    throw new Exception($"Row Fields count is different than number of columns");
                                currentRow = null;
                                readyForNewField = false;
                            }
                            break;
                        case '\'':
                            {
                                if (readyForNewField)
                                {
                                    currentFieldBuilder = new StringBuilder();
                                    currentColumnName = columnNames[currentRow.Count];
                                    isStringField = true;
                                }
                                //else
                                //{
                                //    break;
                                //}
                            }
                            break;
                        case ' ':
                            {

                            }
                            break;
                        case ',':
                            {
                                if (currentRow != null)
                                    readyForNewField = true;
                            }
                            break;
                        case '\r':
                            {

                            }
                            break;
                        case '\n':
                            {

                            }
                            break;
                        case '\t':
                            {

                            }
                            break;
                        default:
                            {
                                if (readyForNewField)
                                {
                                    currentFieldBuilder = new StringBuilder();
                                    currentFieldBuilder.Append(character);
                                    currentColumnName = columnNames[currentRow.Count];
                                    isStringField = false;
                                }
                                else
                                {
                                    if (dataScripts.Count > 0)
                                        return dataScripts;
                                }
                            }
                            break;
                    }
                }
                characterIndex++;
            }

            return dataScripts;
        }

        static void AddFieldToRow(Dictionary<string, object> currentRow, ref string currentColumnName, ref StringBuilder currentFieldBuilder, bool isStringField, ref bool readyForNewField)
        {
            if(isStringField)
            {
                currentRow.Add(currentColumnName, currentFieldBuilder.ToString());
            }
            else
            {
                string allText = currentFieldBuilder.ToString().Trim();
                if (string.Compare(allText, "null", true) == 0)
                {
                    currentRow.Add(currentColumnName, null);
                }
                else
                {
                    Decimal parsedNumber;
                    if (!decimal.TryParse(allText, out parsedNumber))
                        throw new Exception($"Cannot parse number '{allText}'");
                    currentRow.Add(currentColumnName, parsedNumber);
                }
            }
            currentFieldBuilder = null;
            readyForNewField = false;
        }
        
        private static string GenerateMergeScriptToVerify(string schemaName, string tableName, string[] columnNames, List<string> identifierColumns, List<string> columnsToInsert, List<string> columnsToUpdate, List<Dictionary<string, object>> dataScript)
        {
            string template = @"

;with cte_data(#COLUMNS#)
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
#DATAROWS#
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c(#COLUMNS#))
merge	#SCHEMANAME#.#TABLENAME# as t
using	cte_data as s
on		1=1 and #IDENTIFIERCOLUMNS#
#UPDATEQUERY#
#INSERTQUERY#

";
            StringBuilder scriptBuilder = new StringBuilder(template);
            scriptBuilder.Replace("#COLUMNS#", string.Join(",", columnNames));
            scriptBuilder.Replace("#SCHEMANAME#", schemaName);
            scriptBuilder.Replace("#TABLENAME#", tableName);
            scriptBuilder.Replace("#IDENTIFIERCOLUMNS#", string.Join(" and ", identifierColumns.Select(col => $"t.{col} = s.{col}")));
            if (columnsToUpdate != null && columnsToUpdate.Count > 0)
            {
                scriptBuilder.Replace("#UPDATEQUERY#",
    $@"when matched then
	update set
	{string.Join(",", columnsToUpdate.Select(col => $"{col} = s.{col}"))}");
            }
            else
            {
                scriptBuilder.Replace("#UPDATEQUERY#", "");
            }
            scriptBuilder.Replace("#INSERTQUERY#",
$@"when not matched by target then
	insert({string.Join(",", columnsToInsert)})
	values({string.Join(",", columnsToInsert.Select(col => $"s.{col}"))});");

            StringBuilder dataScriptBuilder = new StringBuilder();
            foreach (var dataRow in dataScript)
            {
                if (dataScriptBuilder.Length > 0)
                {
                    dataScriptBuilder.Append(",");
                    dataScriptBuilder.AppendLine();
                }
                dataScriptBuilder.Append("(");
                dataScriptBuilder.Append(string.Join(",", columnNames.Select(col => dataRow[col] == null ? "null" : (dataRow[col] is string ? $"'{dataRow[col]}'" : dataRow[col]))));
                dataScriptBuilder.Append(")");
            }
            scriptBuilder.Replace("#DATAROWS#", dataScriptBuilder.ToString());

            return scriptBuilder.ToString();
        }

    }

    public interface ITestProxy
    {
        string DoSomething(string input);
    }

    public class TestProxyInterceptor : Castle.Core.Interceptor.IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"{DateTime.Now}: called method: {invocation.Method.Name}, called Parameters: {invocation.Arguments[0]}");
            invocation.ReturnValue = "this is returned value";
        }
    }
}
