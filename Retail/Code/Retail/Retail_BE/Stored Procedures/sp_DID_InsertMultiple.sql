
Create PROCEDURE [Retail_BE].[sp_DID_InsertMultiple]
	@DIDs [Retail_BE].[DIDType] Readonly
AS
BEGIN
	INSERT [Retail_BE].[DID] ([Settings], [SourceId]) 
	SELECT [Settings], [SourceId]
	FROM @DIDs
END