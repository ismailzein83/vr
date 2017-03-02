-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_AccBalance].[sp_CarrierFinancialAccount_Update]
	@ID int,
	@CarrierProfileId INT,
	@CarrierAccountId INT,
	@FinancialAccountSettings nvarchar(MAX),
	@BED datetime,
	@EED datetime = NULL
	
AS
BEGIN
	Update [TOneWhS_AccBalance].CarrierFinancialAccount
	Set CarrierProfileId = @CarrierProfileId,
		CarrierAccountId = @CarrierAccountId,
		FinancialAccountSettings = @FinancialAccountSettings,
		BED = @BED,
		EED = @EED
	Where ID = @ID
END