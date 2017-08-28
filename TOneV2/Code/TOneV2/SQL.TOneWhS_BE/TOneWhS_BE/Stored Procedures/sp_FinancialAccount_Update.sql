-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_FinancialAccount_Update]
	@ID int,
	@FinancialAccountSettings nvarchar(MAX),
	@BED datetime,
	@EED datetime = NULL
	
AS
BEGIN
	Update [TOneWhS_BE].FinancialAccount
	Set FinancialAccountSettings = @FinancialAccountSettings,
		BED = @BED,
		EED = @EED
	Where ID = @ID
END