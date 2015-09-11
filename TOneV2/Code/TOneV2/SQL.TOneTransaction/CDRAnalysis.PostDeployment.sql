/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/



MERGE INTO bp.[BPDefinition] AS Target 
USING (VALUES 
	(N'Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput', N'Execute Strategy Process', N'Vanrise.Fzero.FraudAnalysis.BP.ExecuteStrategyProcess, Vanrise.Fzero.FraudAnalysis.BP', N'{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false,"Url":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Normal/ExecuteStrategyProcessInput.html", "ScheduleTemplateURL":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Scheduled/ExecuteStrategyProcessInput_Scheduled.html"}'),
	(N'Vanrise.Fzero.FraudAnalysis.BP.Arguments.NumberProfilingProcessInput', N'Number Profiling Process', N'Vanrise.Fzero.FraudAnalysis.BP.NumberProfilingProcess, Vanrise.Fzero.FraudAnalysis.BP	{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false,"Url":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Normal/NumberProfilingProcessInput.html", "ScheduleTemplateURL":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Scheduled/NumberProfilingProcessInput_Scheduled.html"}'),
    (N'Vanrise.Fzero.FraudAnalysis.BP.Arguments.AssignStrategyCasesProcessInput', N'Assign Strategy Cases Process', N'Vanrise.Fzero.FraudAnalysis.BP.AssignStrategyCasesProcess, Vanrise.Fzero.FraudAnalysis.BP	{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false,"Url":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Normal/AssignStrategyCasesProcessInput.html", "ScheduleTemplateURL":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Scheduled/AssignStrategyCasesProcessInput_Scheduled.html"}'),
	(N'Vanrise.Fzero.FraudAnalysis.BP.Arguments.FindRelatedNumbersProcessInput', N'Find Related Numbers Process', N'Vanrise.Fzero.FraudAnalysis.BP.FindRelatedNumbersProcess, Vanrise.Fzero.FraudAnalysis.BP	{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":10,"RetryOnProcessFailed":false,"Url":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Normal/FindRelatedNumbersProcessInput.html", "ScheduleTemplateURL":"/Client/Modules/FraudAnalysis/Views/ProcessInputTemplate/Scheduled/FindRelatedNumbersProcess_Scheduled.html"}')



) 
AS Source ([Name], [Title], [FQTN], [Config])
ON Target.[Name] = Source.[Name] 
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET	[Title] = Source.[Title],
			[FQTN] = Source.[FQTN],
			[Config]  = Source.[Config]
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([Name], [Title], [FQTN], [Config])
VALUES ([Name], [Title], [FQTN], [Config])
---- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;


--Custom Code:
--Mapper:

--Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch batch = new Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch();
--            batch.CDRs = new List<Vanrise.Fzero.CDRImport.Entities.CDR>();
--            System.IO.StreamReader sr = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data)).StreamReader;
--            while (!sr.EndOfStream)
--            {
--                var i = sr.ReadLine();

--                Vanrise.Fzero.CDRImport.Entities.CDR cdr = new Vanrise.Fzero.CDRImport.Entities.CDR();
--                cdr.MSISDN = i.Substring(145, 20).Trim();
--                cdr.IMSI = i.Substring(125, 20).Trim();
--                cdr.Destination = i.Substring(198, 20).Trim();
--                cdr.CallClass = i.Substring(434, 10).Trim();
--                cdr.SubType = i.Substring(165, 10).Trim();
--                cdr.IMEI = i.Substring(105, 20).Trim();
--                cdr.CellId = i.Substring(252, 22).Trim();
--                cdr.InTrunk = i.Substring(414, 20).Trim();
--                cdr.OutTrunk = i.Substring(394, 20).Trim();


--                DateTime ConnectDateTime;
--                if (DateTime.TryParseExact(i.Substring(221, 14).Trim(), "yyyyMddHHmmss", System.Globalization.CultureInfo.InvariantCulture,
--                                           System.Globalization.DateTimeStyles.None, out ConnectDateTime))
--                    cdr.ConnectDateTime = ConnectDateTime;



--                int callType = 0;
--                if (int.TryParse(i.Substring(102, 3).Trim(), out callType))
--                    cdr.CallType = callType;

--                decimal cellLatitude;
--                if (decimal.TryParse(i.Substring(609, 9).Trim(), out cellLatitude))
--                    cdr.CellLatitude = cellLatitude;


--                decimal durationInSeconds;
--                if (decimal.TryParse(i.Substring(235, 5).Trim(), out durationInSeconds))
--                    cdr.DurationInSeconds = durationInSeconds;


--                decimal upVolume;
--                if (decimal.TryParse(i.Substring(588, 10).Trim(), out upVolume))
--                    cdr.UpVolume = upVolume;


--                decimal cellLongitude;
--                if (decimal.TryParse(i.Substring(618, 9).Trim(), out cellLongitude))
--                    cdr.CellLongitude = cellLongitude;


--                decimal downVolume;
--                if (decimal.TryParse(i.Substring(598, 10).Trim(), out downVolume))
--                    cdr.DownVolume = downVolume;


--                batch.CDRs.Add(cdr);
--            }
--            mappedBatches.Add("CDR Import", batch);

--            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
--            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;

--            return result;




--Custom Code:
--Activator:

--QueueExecutionFlowTree queueFlowTree = new QueueExecutionFlowTree
--           {
--               Activities = new List<BaseExecutionActivity>
--               {                    
--                   new QueueStageExecutionActivity { StageName = "CDR Import",  QueueName = "CDRQueue", QueueTypeFQTN = typeof(ImportedCDRBatch).AssemblyQualifiedName,
--                       QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(CDRImportActivator).AssemblyQualifiedName} }
--               }
--           };




