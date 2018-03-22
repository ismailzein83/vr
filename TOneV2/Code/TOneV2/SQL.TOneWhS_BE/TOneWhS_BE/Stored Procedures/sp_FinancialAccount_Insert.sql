-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_FinancialAccount_Insert]
	@CarrierProfileId INT,
	@CarrierAccountId INT,
	@FinancialAccountDefinitionId uniqueidentifier,
	@FinancialAccountSettings nvarchar(MAX),
	@BED datetime,
	@EED datetime = NULL,
	@CreatedBy int,
	@LastModifiedBy int,
	@Id int out
AS
BEGIN
	Insert into [TOneWhS_BE].FinancialAccount(CarrierProfileId, CarrierAccountId,FinancialAccountDefinitionId, FinancialAccountSettings, BED, EED, CreatedBy, LastModifiedBy, LastModifiedTime)
	Values (@CarrierProfileId, @CarrierAccountId,@FinancialAccountDefinitionId, @FinancialAccountSettings, @BED, @EED, @CreatedBy, @LastModifiedBy, GETDATE() )
	Set @Id = SCOPE_IDENTITY()
END