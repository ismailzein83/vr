-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_ExcludedItems_GetAll] 
	@ProcessInstanceId bigint
AS
BEGIN
	SELECT ItemID,ITemType,ItemName,Description,ParentId,ProcessInstanceId
    FROM TOneWhS_Sales.RP_ExcludedItems
	WHERE ProcessInstanceId = @ProcessInstanceId AND ParentId is null
	
END