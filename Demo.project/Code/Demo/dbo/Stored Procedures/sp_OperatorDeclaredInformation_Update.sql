
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
	@DestinationGroup int,
	@Volume int,
	@AmountType int,
	@Attachment bigint, 
	@Notes varchar(max)
AS
BEGIN


	Update dbo.OperatorDeclaredInformation
		Set OperatorID = @OperatorID,
		DestinationGroup = @DestinationGroup,
		Volume = @Volume,
		AmountType = @AmountType,
		FromDate = @FromDate,
		Notes = @Notes,
		Attachment = @Attachment,
		ToDate = @ToDate
	Where ID = @ID
END