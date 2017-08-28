-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_BE].[sp_FinancialAccount_Insert]
	@CarrierProfileId INT,
	@CarrierAccountId INT,
	@FinancialAccountDefinitionId uniqueidentifier,
	@FinancialAccountSettings nvarchar(MAX),
	@BED datetime,
	@EED datetime = NULL,
	@Id int out
AS
BEGIN
	Insert into [TOneWhS_BE].FinancialAccount(CarrierProfileId, CarrierAccountId,FinancialAccountDefinitionId, FinancialAccountSettings, BED, EED  )
	Values (@CarrierProfileId, @CarrierAccountId,@FinancialAccountDefinitionId, @FinancialAccountSettings, @BED, @EED )
	Set @Id = SCOPE_IDENTITY()
END