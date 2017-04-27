-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_HoldRequest_Delete]
@HoldRequestId bigint
AS
BEGIN
	Delete From [queue].[HoldRequest] where ID = @HoldRequestId
END