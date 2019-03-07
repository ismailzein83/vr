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

--[genericdata].[DataStore]-------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('F83A615A-6514-4B9D-9E10-5C5EC4234069','CDR Data Store','{"$type":"Vanrise.GenericData.SQLDataStorage.SQLDataStoreSettings, Vanrise.GenericData.SQLDataStorage","ConfigId":"2aeec2de-ec44-4698-aaef-8e9dbf669d1e","ConnectionStringName":"Mediation_CDR_DBConnString","IsRemoteDataStore":false}'),
('B092FC56-5D9D-4A41-B385-0BFB24C72CBA','CDR File Store','{"$type":"Vanrise.GenericData.FileDataStorage.FileDataStoreSettings, Vanrise.GenericData.FileDataStorage","ConfigId":"DB1224D0-CC3D-481D-9163-E4E7C5231D00","ConnectionStringName":"Mediation_CDR_DBConnString"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[genericdata].[DataStore] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);