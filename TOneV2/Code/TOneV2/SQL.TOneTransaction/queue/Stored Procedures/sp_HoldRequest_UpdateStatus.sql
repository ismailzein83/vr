-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_HoldRequest_UpdateStatus]
@HoldRequestId bigint,
@Status int
AS
BEGIN
	Update [queue].[HoldRequest]
	set [Status]= @Status
	where ID = @HoldRequestId
END