-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_AccBalance].[sp_CarrierFinancialAccount_Insert]
	@CarrierProfileId INT,
	@CarrierAccountId INT,
	@FinancialAccountSettings nvarchar(MAX),
	@BED datetime,
	@EED datetime = NULL,
	@Id int out
AS
BEGIN
	Insert into [TOneWhS_AccBalance].CarrierFinancialAccount(CarrierProfileId, CarrierAccountId, FinancialAccountSettings, BED, EED  )
	Values (@CarrierProfileId, @CarrierAccountId, @FinancialAccountSettings, @BED, @EED )
	Set @Id = SCOPE_IDENTITY()
END