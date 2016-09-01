


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_OperatorConfiguration_Update]
	@ID int,
	@OperatorID int,
	@Volume int,
	@CDRDirection int,
	@Percentage float,
	@Amount float,
	@Currency int,
	@FromDate datetime, 
	@ToDate datetime,
	@Notes nvarchar(max),
	@ServiceSubTypeSettings	nvarchar(MAX), 
	@DestinationGroup int,
	@InterconnectOperator int
AS
BEGIN


	Update dbo.OperatorConfiguration
		Set OperatorID = @OperatorID,
		Volume = @Volume,
		CDRDirection =@CDRDirection ,
		Percentage =@Percentage ,
		Amount =@Amount ,
		Currency =@Currency ,
		FromDate=@FromDate , 
		ToDate=@ToDate ,
		Notes=@Notes,
		ServiceSubTypeSettings=@ServiceSubTypeSettings,
		DestinationGroup=@DestinationGroup,
		InterconnectOperator=@InterconnectOperator

	Where ID = @ID
END