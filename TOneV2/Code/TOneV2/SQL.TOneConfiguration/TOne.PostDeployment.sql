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

MERGE INTO [BI].[SchemaConfiguration] AS Target 
USING (VALUES 
	(N'ACD', N'ACD', 1, N'{"ColumnName":"[Measures].[ACD]","Expression":"","RequiredPermissions":"TOne/Business Intelligence Module/Billing Module/Billing Statistics:View"}'),
(N'COST', N'COST', 1, N'{"ColumnName":"[Measures].[Cost Net]","Expression":"","RequiredPermissions":""}'),
(N'SALE', N'SALE', 1, N'{"ColumnName":"[Measures].[Sale Net]","Expression":"","RequiredPermissions":""}'),
(N'DURATION_IN_MINUTES', N'DURATION IN MINUTES', 1, N'{"ColumnName":"[Measures].[Duration In Minutes]","Expression":"","RequiredPermissions":""}'),
(N'PROFIT', N'PROFIT', 1, N'{"ColumnName":"[Measures].[Profit_CALC]","Expression":"MEMBER [Measures].[Profit_CALC]  AS ([Measures].[Sale Net] - [Measures].[Cost Net]),","RequiredPermissions":""}'),
(N'SUCCESSFUL_ATTEMPTS', N'SUCCESSFUL ATTEMPTS', 1, N'{"ColumnName":"[Measures].[SuccessfulAttempts]","Expression":"","RequiredPermissions":""}'),
(N'PDD', N'PDD', 1, N'{"ColumnName":"[Measures].[PDD]","Exepression":"","RequiredPermissions":""}'),
(N'Supplier', N'Supplier', 0, N'{"ColumnID":"[SupplierAccounts].[Carrier Account ID]","ColumnName":"[SupplierAccounts].[Profile Name]","Expression":""}'),
(N'Customer', N'Customer', 0, N'{"ColumnID":"[CustomerAccounts].[Carrier Account ID]","ColumnName":"[CustomerAccounts].[Profile Name]","Expression":""}'),
(N'SaleZone', N'SaleZone', 0, N'{"ColumnID":"[SaleZones].[Zone ID]","ColumnName":"[SaleZones].[Z One Name]","Expression":""}'),
(N'BWASR', N'BWASR', 1, N'{"ColumnName":"[Measures].[BWASR]","Expression":"","RequiredPermissions":""}'),
(N'Facttable Count', N'Facttable Count', 1, N'{"ColumnName":"[Measures].[Facttable Count]","Expression":"","RequiredPermissions":""}'),
(N'Duration In Minutes', N'Duration In Minutes', 1, N'{"ColumnName":"[Measures].[Duration In Minutes]","Expression":"","RequiredPermissions":""}'),
(N'ICABR', N'ICABR', 1, N'{"ColumnName":"[Measures].[ICABR]","Expression":"","RequiredPermissions":""}'),
(N'ICASR', N'ICASR', 1, N'{"ColumnName":"[Measures].[ICASR]","Expression":"","RequiredPermissions":""}'),
(N'ICCCR', N'ICCCR', 1, N'{"ColumnName":"[Measures].[ICCCR]","Expression":"","RequiredPermissions":""}'),
(N'ICNER', N'ICNER', 1, N'{"ColumnName":"[Measures].[ICNER]","Expression":"","RequiredPermissions":""}'),
(N'IIR', N'IIR', 1, N'{"ColumnName":"[Measures].[IIR]","Expression":"","RequiredPermissions":""}'),
(N'MHT', N'MHT', 1, N'{"ColumnName":"[Measures].[MHT]","Expression":"","RequiredPermissions":""}'),
(N'MHT-Per-Call', N'MHT-Per-Call', 1, N'{"ColumnName":"[Measures].[MHT-Per-Call]","Expression":"","RequiredPermissions":""}'),
(N'OGABR', N'OGABR', 1, N'{"ColumnName":"[Measures].[OGABR]","Expression":"","RequiredPermissions":""}'),
(N'OGASR', N'OGASR', 1, N'{"ColumnName":"[Measures].[OGASR]","Expression":"","RequiredPermissions":""}'),
(N'OGCCR', N'OGCCR', 1, N'{"ColumnName":"[Measures].[OGCCR]","Expression":"","RequiredPermissions":""}'),
(N'OGNER', N'OGNER', 1, N'{"ColumnName":"[Measures].[OGNER]","Expression":"","RequiredPermissions":""}'),
(N'OIR', N'OIR', 1, N'{"ColumnName":"[Measures].[OIR]","Expression":"","RequiredPermissions":""}'),
(N'PDD-Per-Call', N'PDD-Per-Call', 1, N'{"ColumnName":"[Measures].[PDD-Per-Call]","Expression":"","RequiredPermissions":""}'),
(N'Seizures', N'Seizures', 1, N'{"ColumnName":"[Measures].[Seizures]","Expression":"","RequiredPermissions":""}'),
(N'Switch', N'Switch', 0, N'{"ColumnID":"[Switch].[Switch ID]","ColumnName":"[Switch].[Name]","Expression":""}'),
(N'SaleRate', N'SaleRate', 0, N'{"ColumnID":"[SaleRate].[Rate ID]","ColumnName":"[SaleRate].[Rate]","Expression":""}'),
(N'CostZones', N'CostZones', 0, N'{"ColumnID":"[CostZones].[ID]","ColumnName":"[CostZones].[Z One Name]","Expression":""}')

) 
AS Source ([Name], [DisplayName], [Type], [Configuration])
ON Target.[Name] = Source.[Name] 
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET	[DisplayName] = Source.[DisplayName],
			[Type] = Source.[Type],
			[Configuration]  = Source.[Configuration]
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT ([Name], [DisplayName], [Type], [Configuration])
VALUES ([Name], [DisplayName], [Type], [Configuration])
---- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;