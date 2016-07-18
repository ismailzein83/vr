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
--check if Datasource already defined
if exists (select 1 from [queue].[ExecutionFlowDefinition] where ID=-102)
BEGIN
 select 'Data source already defined, you may loose updated mapping and connection string. if you need to override please remove this condition.'
 RETURN
 END