﻿/*
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


--[bp].[BPDefinition]----------------------5001 to 6000---------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[FQTN],[Config])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('7E7153B7-2781-4A53-B3FB-8E451AA21046','Vanrise.NumberingPlan.BP.Arguments.CodePreparationInput','Numbering Plan','Vanrise.NumberingPlan.BP.NumberingPlan, Vanrise.NumberingPlan.BP','{"$type":"Vanrise.BusinessProcess.Entities.BPConfiguration, Vanrise.BusinessProcess.Entities","MaxConcurrentWorkflows":1,"IsPersistable":false,"HasChildProcesses":false,"HasBusinessRules":true,"NotVisibleInManagementScreen":false,"Security":{"$type":"Vanrise.BusinessProcess.Entities.BPDefinitionSecurity, Vanrise.BusinessProcess.Entities","View":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"B615F6EF-107E-469D-8DAE-407F208CF9B7","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["View"]}}]}},"StartNewInstance":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"B615F6EF-107E-469D-8DAE-407F208CF9B7","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}},"ScheduleTask":{"$type":"Vanrise.Security.Entities.RequiredPermissionSettings, Vanrise.Security.Entities","Entries":{"$type":"System.Collections.Generic.List`1[[Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities]], mscorlib","$values":[{"$type":"Vanrise.Security.Entities.RequiredPermissionEntry, Vanrise.Security.Entities","EntityId":"B615F6EF-107E-469D-8DAE-407F208CF9B7","PermissionOptions":{"$type":"System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib","$values":["Start Process"]}}]}}}}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[FQTN],[Config]))
merge	[bp].[BPDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[FQTN] = s.[FQTN],[Config] = s.[Config]
when not matched by target then
	insert([ID],[Name],[Title],[FQTN],[Config])
	values(s.[ID],s.[Name],s.[Title],s.[FQTN],s.[Config]);
----------------------------------------------------------------------------------------------------
END

--[bp].[BPTaskType]------------------------------------------------------
----------------------------------------------------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('3D917365-FE65-4FAD-AAF8-19D7354E2A3D','Vanrise.NumberingPlan.BP.Arguments.PreviewTaskData','{"$type":"Vanrise.BusinessProcess.Entities.BPTaskTypeSettings, Vanrise.BusinessProcess.Entities","Editor":"/Client/Modules/VR_NumberingPlan/Views/NumberingPlanPreview.html","AutoOpenTask":true}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[bp].[BPTaskType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);
END


delete from [bp].[BPBusinessRuleAction] where BusinessRuleDefinitionId IN ('D468D1B7-CED3-4CAC-A88E-3512586DD908')
delete from [bp].[BPBusinessRuleDefinition] where ID IN ('D468D1B7-CED3-4CAC-A88E-3512586DD908')

--[bp].[BPBusinessRuleDefinition]-------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
Begin
set nocount on;
;with cte_data([ID],[Name],[BPDefintionId],[Settings],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('6AFC72ED-E87A-428D-8B48-C7E648E1B182','VR_NP_ValidateZones','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Code Group","Condition":{"$type":"Vanrise.NumberingPlan.Business.CodeGroupCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',1),
('CD49FD79-B0AC-4D73-8CE5-9273CC99DF32','VR_NP_ValidateZones','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Multiple Country","Condition":{"$type":"Vanrise.NumberingPlan.Business.MultipleCountryCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',2),
('ACC29006-2370-46CC-B81F-05898E5C5AE5','VR_NP_ValidateZones','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Zone has Same Code more than one time","Condition":{"$type":"Vanrise.NumberingPlan.Business.SameCodeInSameZoneCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',3),
('1C1A42CF-56D4-49BE-A0C7-08474E42E91E','VR_NP_ValidateAfterProcessing','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":" Move Code To a Pending Effective Zone","Condition":{"$type":"Vanrise.NumberingPlan.Business.MoveCodeToPendingEffectiveZoneCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',2),
('90C4A588-2E14-4099-8CA4-16DE9AB2F6AF','VR_NP_ValidateUICodesZones','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Zone has no changes","Condition":{"$type":"Vanrise.NumberingPlan.Business.ZoneWithoutChangesCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["fbfe2b36-12f6-40c1-8163-26cfe2d23501"]}',3),
('8B4F3FE1-5D70-4C28-A266-2323802E64B2','VR_NP_ValidateAfterProcessing','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Can not close pending effective code","Condition":{"$type":" Vanrise.NumberingPlan.Business.ClosePendingEffectiveCodeCondition,  Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',4),
('BA70CAA0-5547-4B49-BC55-4B13665DCE19','VR_NP_ValidateCodesZones','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Numeric Code","Condition":{"$type":"Vanrise.NumberingPlan.Business.MissingCodeCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',6),
('7B292659-AD22-417B-BDD2-63C829F7DFF4','VR_NP_ValidateCodesZones','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Missing Status","Condition":{"$type":"Vanrise.NumberingPlan.Business.MissingStatusCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',7),
('95AA0C9E-310D-4FAB-86F3-65B296C922A3','VR_NP_ValidateAfterProcessing','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"All Codes In Zone Closed","Condition":{"$type":"Vanrise.NumberingPlan.Business.AllCodesInZoneClosedCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["FBFE2B36-12F6-40C1-8163-26CFE2D23501"]}',8),
('F3A627E1-F196-4A8D-9A29-6AA6A73C57A2','VR_NP_ValidateAfterProcessingCodes','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Close Code Not Exist And Effective","Condition":{"$type":"Vanrise.NumberingPlan.Business.CloseCodeNotExistAndEffectiveCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',9),
('BDA513DA-A7F0-4A9F-BE9F-7978B12EE8D8','VR_NP_ValidateCodesZones','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Missing Zone","Condition":{"$type":"Vanrise.NumberingPlan.Business.MissingZoneCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',10),
('A5643CFF-4078-498A-BA54-8B652D83B52D','VR_NP_ValidateAfterProcessing','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Can not close pending closed code","Condition":{"$type":"Vanrise.NumberingPlan.Business.ClosePendingClosedCodeCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',11),
('B38A1A28-5F19-4D41-AEE2-8C8BD3F17052','VR_NP_ValidateCodesZones','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Invalid Status","Condition":{"$type":"Vanrise.NumberingPlan.Business.InvalidStatusCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',12),
('74959162-8F91-4CF8-BBA4-B30856268121','VR_NP_ValidateAfterProcessing','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Code Effective Before Zone Condition","Condition":{"$type":"Vanrise.NumberingPlan.Business.CodeEffectiveBeforeZoneCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715F7F90-2C23-4185-AEB8-EDA947DE3978"]}',14),
('0453480B-5C5A-4CA1-ABAE-BC60A2D7F09C','VR_NP_ValidateAfterProcessing','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Wrong Move Condition","Condition":{"$type":"Vanrise.NumberingPlan.Business.WrongMoveCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',15),
('59081EF0-F4BD-4C1D-8F02-D74BF0A1E7CA','VR_NP_ValidateAfterProcessing','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Code With Different Country Code Group","Condition":{"$type":"Vanrise.NumberingPlan.Business.CodeWithDifferentCountryCodeGroupCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',17),
('A1092F4F-FEF5-4D16-8F58-DA8AF84228C8','VR_NP_ValidateCountries','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Same code in different zones","Condition":{"$type":"Vanrise.NumberingPlan.Business.SameCodeInDifferentZoneCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',18),
('F844B099-69B3-4516-AC94-DD43B173FE80','VR_NP_ValidateCountries','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Changes For Empty Country","Condition":{"$type":"Vanrise.NumberingPlan.Business.ChangesInPastCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',19),
('9F784154-65D7-4C67-8018-DDA2FE4F4D66','VR_NP_ValidateAfterProcessingCodes','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Move Code Not Exist And Effective","Condition":{"$type":"Vanrise.NumberingPlan.Business.MoveCodeNotExistAndEffectiveCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',20),
('120760D6-3698-43BA-B9B1-E30F0A8773A3','VR_NP_ValidateAfterProcessing','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":"Add or Move Code To a Pending Zone","Condition":{"$type":"Vanrise.NumberingPlan.Business.AddMoveCodeToPendingClosedZoneCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',21),
('89622DA0-BCD8-41D5-A065-047CA34C142A','VR_NP_ValidateAfterProcessing','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":" Move Closed Code To New Zonee","Condition":{"$type":"Vanrise.NumberingPlan.Business.MoveClosedCodeToNewZoneCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',10),
('A246FCEF-2C58-487D-A3AC-4858C4C3DED7','VR_NP_ValidateAfterProcessing','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":" Move Pending Effective Code","Condition":{"$type":"Vanrise.NumberingPlan.Business.MovePendingEffectiveCodeCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',2),
('8C392B45-7E83-41AC-A44B-ED68A085B257','VR_NP_ValidateAfterProcessing','7E7153B7-2781-4A53-B3FB-8E451AA21046','{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleSettings, Vanrise.BusinessProcess.Entities","Description":" Move Pending Closed Code","Condition":{"$type":"Vanrise.NumberingPlan.Business.MovePendingClosedCodeCondition, Vanrise.NumberingPlan.Business"},"ActionTypes":["715f7f90-2c23-4185-aeb8-eda947de3978"]}',5)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[BPDefintionId],[Settings],[Rank]))
merge	[bp].[BPBusinessRuleDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[BPDefintionId] = s.[BPDefintionId],[Settings] = s.[Settings],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[BPDefintionId],[Settings],[Rank])
	values(s.[ID],s.[Name],s.[BPDefintionId],s.[Settings],s.[Rank]);
End

--[bp].[BPBusinessRuleAction]-----------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
Begin
set nocount on;
;with cte_data([Settings],[BusinessRuleDefinitionId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','ACC29006-2370-46CC-B81F-05898E5C5AE5'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','1C1A42CF-56D4-49BE-A0C7-08474E42E91E'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.WarningItemAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"fbfe2b36-12f6-40c1-8163-26cfe2d23501"}}','90C4A588-2E14-4099-8CA4-16DE9AB2F6AF'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','8B4F3FE1-5D70-4C28-A266-2323802E64B2'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','BA70CAA0-5547-4B49-BC55-4B13665DCE19'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','7B292659-AD22-417B-BDD2-63C829F7DFF4'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.InformationAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"72c926f1-d019-408f-84af-6613d2033473"}}','95AA0C9E-310D-4FAB-86F3-65B296C922A3'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','F3A627E1-F196-4A8D-9A29-6AA6A73C57A2'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','BDA513DA-A7F0-4A9F-BE9F-7978B12EE8D8'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','A5643CFF-4078-498A-BA54-8B652D83B52D'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','B38A1A28-5F19-4D41-AEE2-8C8BD3F17052'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','CD49FD79-B0AC-4D73-8CE5-9273CC99DF32'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','74959162-8F91-4CF8-BBA4-B30856268121'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','0453480B-5C5A-4CA1-ABAE-BC60A2D7F09C'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','6AFC72ED-E87A-428D-8B48-C7E648E1B182'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','59081EF0-F4BD-4C1D-8F02-D74BF0A1E7CA'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','A1092F4F-FEF5-4D16-8F58-DA8AF84228C8'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','F844B099-69B3-4516-AC94-DD43B173FE80'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','9F784154-65D7-4C67-8018-DDA2FE4F4D66'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','120760D6-3698-43BA-B9B1-E30F0A8773A3'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','8C392B45-7E83-41AC-A44B-ED68A085B257'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','89622DA0-BCD8-41D5-A065-047CA34C142A'),
('{"$type":"Vanrise.BusinessProcess.Entities.BPBusinessRuleActionSettings, Vanrise.BusinessProcess.Entities","Action":{"$type":"Vanrise.BusinessProcess.StopExecutionAction, Vanrise.BusinessProcess","BPBusinessRuleActionTypeId":"715f7f90-2c23-4185-aeb8-eda947de3978"}}','A246FCEF-2C58-487D-A3AC-4858C4C3DED7')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Settings],[BusinessRuleDefinitionId]))
merge	[bp].[BPBusinessRuleAction] as t
using	cte_data as s
on		1=1 and t.[BusinessRuleDefinitionId] = s.[BusinessRuleDefinitionId]
when matched then
	update set
	[Settings] = s.[Settings],[BusinessRuleDefinitionId] = s.[BusinessRuleDefinitionId]
when not matched by target then
	insert([Settings],[BusinessRuleDefinitionId])
	values(s.[Settings],s.[BusinessRuleDefinitionId]);
End

--[sec].[Module]------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
--('E7855563-9173-47F0-A8E7-4C47CD2A1F42','Numbering Plan','Numbering Plan',null,'/Client/Images/menu-icons/Business Entities.png',11,1),
('e7855563-9173-47f0-a8e7-4c47cd2a1f42','Voice Entites',null,'e73c4aba-fd03-4137-b047-f3fb4f7eed03',null,3,0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic]))
merge	[sec].[Module] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Url] = s.[Url],[ParentId] = s.[ParentId],[Icon] = s.[Icon],[Rank] = s.[Rank],[AllowDynamic] = s.[AllowDynamic]
when not matched by target then
	insert([ID],[Name],[Url],[ParentId],[Icon],[Rank],[AllowDynamic])
	values(s.[ID],s.[Name],s.[Url],s.[ParentId],s.[Icon],s.[Rank],s.[AllowDynamic]);

--[sec].[View]--------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('1F11C51B-1EDB-4559-8EE2-25A44330CCA4','Selling Number Plans','Selling Number Plans','#/view/VR_NumberingPlan/Views/SellingNumberPlanManagement'	,'e7855563-9173-47f0-a8e7-4c47cd2a1f42','VR_NumberingPlan/SellingNumberPlan/GetFilteredSellingNumberPlans',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',5),
('11927F62-D139-452B-B23B-14EC58F87012','Code Groups','Code Groups','#/view/VR_NumberingPlan/Views/CodeGroup/CodeGroupManagement'					,'e7855563-9173-47f0-a8e7-4c47cd2a1f42','VR_NumberingPlan/CodeGroup/GetFilteredCodeGroups',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',10),
('29FE98DA-91D7-4442-8A35-3A721CD5656A','Sale Zones','Sale Zones','#/view/VR_NumberingPlan/Views/SaleZone/SaleZoneManagement'						,'e7855563-9173-47f0-a8e7-4c47cd2a1f42','VR_NumberingPlan/SaleZone/GetFilteredSaleZones',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',15),
('90331F0A-FB51-4DCF-8261-98D07B579938','Sale Codes','Sale Codes','#/view/VR_NumberingPlan/Views/SaleCode/SaleCodeManagement'						,'e7855563-9173-47f0-a8e7-4c47cd2a1f42','VR_NumberingPlan/SaleCode/GetFilteredSaleCodes',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',20),
('05CD2EE2-7374-4296-A532-CC273EE1C540','Numbering Plan','Numbering Plan','#/view/VR_NumberingPlan/Views/NumberingPlanManagement'					,'e7855563-9173-47f0-a8e7-4c47cd2a1f42','VR_NumberingPlan/CodePreparation/CheckCodePreparationState',null,null,null,'372ED3CB-4B7B-4464-9ABF-59CD7B08BD23',25)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);

--[common].[Setting]--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('A4BBE063-AE8E-4AE3-AD42-5959E198E51B','Numbering Plan','VR_NumberingPlan','Business Entities','{"Editor":"vr-np-settings-editor"}','{"$type":"Vanrise.NumberingPlan.Entities.NPSettingsData, Vanrise.NumberingPlan.Entities","EffectiveDateOffset":7}',0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical]))
merge	[common].[Setting] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name],[Type] = s.[Type],[Category] = s.[Category],[Settings] = s.[Settings],[Data] = s.[Data],[IsTechnical] = s.[IsTechnical]
when not matched by target then
	insert([ID],[Name],[Type],[Category],[Settings],[Data],[IsTechnical])
	values(s.[ID],s.[Name],s.[Type],s.[Category],s.[Settings],s.[Data],s.[IsTechnical]);

--[sec].[SystemAction]------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Name],[RequiredPermissions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('VR_NumberingPlan/SellingNumberPlan/GetSellingNumberPlans',null),
('VR_NumberingPlan/SellingNumberPlan/GetSellingNumberPlan',null),
('VR_NumberingPlan/SellingNumberPlan/GetMasterSellingNumberPlan',null),
('VR_NumberingPlan/SellingNumberPlan/GetFilteredSellingNumberPlans','VR_NumberingPlan_SellingNumberPlan: View'),
('VR_NumberingPlan/SellingNumberPlan/AddSellingNumberPlan','VR_NumberingPlan_SellingNumberPlan: Add'),
('VR_NumberingPlan/SellingNumberPlan/UpdateSellingNumberPlan','VR_NumberingPlan_SellingNumberPlan: Edit'),
('VR_NumberingPlan/CodeGroup/GetFilteredCodeGroups','VR_NumberingPlan_CodeGroup: View'),
('VR_NumberingPlan/CodeGroup/GetAllCodeGroups',null),
('VR_NumberingPlan/CodeGroup/GetCodeGroup',null),
('VR_NumberingPlan/CodeGroup/AddCodeGroup','VR_NumberingPlan_CodeGroup: Add'),
('VR_NumberingPlan/CodeGroup/UpdateCodeGroup','VR_NumberingPlan_CodeGroup: Edit'),
('VR_NumberingPlan/CodeGroup/UploadCodeGroupList','VR_NumberingPlan_CodeGroup : Add'),
('VR_NumberingPlan/CodeGroup/DownloadCodeGroupListTemplate',null),
('VR_NumberingPlan/CodeGroup/DownloadCodeGroupLog',null),
('VR_NumberingPlan/SaleZone/GetFilteredSaleZones','VR_NumberingPlan_SaleZoneAndCode: View'),
('VR_NumberingPlan/SaleZone/GetSaleZone',null),
('VR_NumberingPlan/SaleZone/GetSellingNumberPlanIdBySaleZoneId',null),
('VR_NumberingPlan/SaleZone/GetSaleZoneGroupTemplates',null),
('VR_NumberingPlan/SaleCode/GetFilteredSaleCodes','VR_NumberingPlan_SaleZoneAndCode: View'),
('VR_NumberingPlan/CodePreparation/DownloadImportCodePreparationTemplate',null),
('VR_NumberingPlan/CodePreparation/GetChanges',null),
('VR_NumberingPlan/CodePreparation/SaveNewZone',null),
('VR_NumberingPlan/CodePreparation/SaveNewCode',null),
('VR_NumberingPlan/CodePreparation/MoveCodes',null),
('VR_NumberingPlan/CodePreparation/CloseCodes',null),
('VR_NumberingPlan/CodePreparation/CloseZone',null),
('VR_NumberingPlan/CodePreparation/RenameZone',null),
('VR_NumberingPlan/CodePreparation/GetZoneItems',null),
('VR_NumberingPlan/CodePreparation/CheckCodePreparationState','VR_NumberingPlan_NumberingPlan: Start Process'),
('VR_NumberingPlan/CodePreparation/CancelCodePreparationState',null),
('VR_NumberingPlan/CodePreparation/GetCodeItems',null),
('VR_NumberingPlan/CodePreparation/GetCPSettings',null)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Name],[RequiredPermissions]))
merge	[sec].[SystemAction] as t
using	cte_data as s
on		1=1 and t.[Name] = s.[Name]
when matched then
	update set
	[Name] = s.[Name],[RequiredPermissions] = s.[RequiredPermissions]
when not matched by target then
	insert([Name],[RequiredPermissions])
	values(s.[Name],s.[RequiredPermissions]);


--[sec].[BusinessEntity]----------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('5B732E53-C47C-4885-B516-35E60BAF214C','VR_NumberingPlan_CodeGroup','Code Group'					,'8FAF6AA2-6C00-4C48-8EBF-68B87D2DC493',0,'["View","Add","Edit"]'),

('DB827E90-C23E-4AB6-A725-4EB83C21F5FE','VR_NumberingPlan_SellingNumberPlan','Selling Number Plan'	,'D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',0,'["View","Add","Edit"]'),
('B84ACE6E-3A57-4582-81D7-A7F47FCC23C8','VR_NumberingPlan_SaleZoneAndCode','Sale Zones & Codes'		,'D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',0,'["View"]'),
('B615F6EF-107E-469D-8DAE-407F208CF9B7','VR_NumberingPlan_NumberingPlan','Numbering Plan'			,'D9666AEA-9517-4DC5-A3D2-D074B2B99A1C',0,'["View", "Start Process"]')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions]))
merge	[sec].[BusinessEntity] as t
using	cte_data as s
on		1=1 and t.[Id] = s.[Id]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[ModuleId] = s.[ModuleId],[BreakInheritance] = s.[BreakInheritance],[PermissionOptions] = s.[PermissionOptions]
when not matched by target then
	insert([Id],[Name],[Title],[ModuleId],[BreakInheritance],[PermissionOptions])
	values(s.[Id],s.[Name],s.[Title],s.[ModuleId],s.[BreakInheritance],s.[PermissionOptions]);

--to be updated wtih new records related to VR_NumberingPlan
--[genericdata].[BusinessEntityDefinition]----------------------------------------------------------
BEGIN
set nocount on;
;with cte_data([ID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('10740F30-5A20-4718-B5AF-0E2E160B21C2','VR_NumberingPlan_SaleZone','Sale Zone'							,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"vr-np-salezone-selector","ManagerFQTN":"Vanrise.NumberingPlan.Business.SaleZoneManager, Vanrise.NumberingPlan.Business","IdType":"System.Int64"}'),
('2EC2FB2D-2343-40EB-B72A-9A90F99DF0C7','VR_NumberingPlan_SellingNumberPlan','Selling Number Plan'		,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"vr-np-sellingnumberplan-selector","GroupSelectorUIControl":"","ManagerFQTN":"Vanrise.NumberingPlan.Business.SellingNumberPlanManager, Vanrise.NumberingPlan.Business","IdType":"System.Int32"}'),
('F650D523-7ADB-4787-A2F6-C13168F7E8F7','VR_NumberingPlan_SaleZoneMasterPlan','Master Sale Zone'		,'{"$type":"Vanrise.GenericData.Entities.StaticBEDefinitionSettings, Vanrise.GenericData.Entities","SelectorUIControl":"vr-np-salezone-masterplan-selector","ManagerFQTN":"Vanrise.NumberingPlan.Business.SaleZoneManager, Vanrise.NumberingPlan.Business","IdType":"System.Int64"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Settings]))
merge	[genericdata].[BusinessEntityDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[Settings]);
----------------------------------------------------------------------------------------------------
END

--[VR_NumberingPlan].[SellingNumberPlan]------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [VR_NumberingPlan].[SellingNumberPlan] on;
;with cte_data([ID],[Name])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Default')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name]))
merge	[VR_NumberingPlan].[SellingNumberPlan] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
--when matched then
--	update set
--	[Name] = s.[Name]
when not matched by target then
	insert([ID],[Name])
	values(s.[ID],s.[Name]);
set identity_insert [VR_NumberingPlan].[SellingNumberPlan] off;