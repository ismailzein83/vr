-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_HoldRequest_DeleteByBPInstanceId]
	@BPInstanceId bigint
AS
BEGIN
	Delete From [queue].[HoldRequest] where BPInstanceID = @BPInstanceId
	Update [queue].[HoldRequest] set BPInstanceID = BPInstanceID
END