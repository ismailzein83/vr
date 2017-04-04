-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_DataRecordType_SetCacheExpired]
	
AS
BEGIN
	Update genericdata.[DataRecordType] set Name = Name
	where ID = (select Top 1 ID from genericdata.[DataRecordType])
END