
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_NationalNumberingPlan_Update]
	@ID int,
	@OperatorID int,
	@FromDate datetime,
	@ToDate datetime,
	@Settings nvarchar(MAX)
AS
BEGIN


	Update dbo.NationalNumberingPlan
	Set OperatorID = @OperatorID,
		FromDate = @FromDate,
		ToDate = @ToDate,
		Settings=@Settings
	Where ID = @ID
END