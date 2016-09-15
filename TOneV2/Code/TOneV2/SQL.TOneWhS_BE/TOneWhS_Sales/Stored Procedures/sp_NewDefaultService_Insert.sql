-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_Sales.sp_NewDefaultService_Insert
	@ID bigint,
	@ProcessInstanceID bigint,
	@Services nvarchar(max),
	@BED datetime,
	@EED datetime = null
AS
BEGIN
	insert into [TOneWhS_Sales].[RP_DefaultService_New] (ID, ProcessInstanceID, [Services], BED, EED)
	values (@ID, @ProcessInstanceID, @Services, @BED, @EED)
END