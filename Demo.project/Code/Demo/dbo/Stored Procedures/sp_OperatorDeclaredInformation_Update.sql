
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_OperatorDeclaredInformation_Update]
	@ID int,
	@OperatorID int,
	@FromDate datetime,
	@ToDate datetime,
	@ZoneID bigint,
	@Volume int,
	@AmountType int,
	@Attachment bigint, 
	@Notes varchar(max)
AS
BEGIN


	Update dbo.OperatorDeclaredInformation
		Set OperatorID = @OperatorID,
		ZoneID = @ZoneID,
		Volume = @Volume,
		AmountType = @AmountType,
		FromDate = @FromDate,
		Notes = @Notes,
		Attachment = @Attachment,
		ToDate = @ToDate
	Where ID = @ID
END