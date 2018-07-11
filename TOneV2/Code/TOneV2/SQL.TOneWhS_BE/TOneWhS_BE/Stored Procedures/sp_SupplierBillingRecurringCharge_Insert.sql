
CREATE PROCEDURE[TOneWhS_BE].[sp_SupplierBillingRecurringCharge_Insert]

@FinancialAccountId int,
@RecurringChargeId bigint,
@InvoiceId bigint,
@Amount decimal(22,6),
@From   datetime,
@To datetime,
@CurrencyId int,
@CreatedBy int

AS
BEGIN




	SET NOCOUNT ON;
insert into [TOneWhS_BE].[SupplierBillingRecurringCharge](FinancialAccountId,RecurringChargeId,InvoiceId,Amount,[From],[To],CurrencyId,CreatedBy) values(@FinancialAccountId,@RecurringChargeId,@InvoiceId,@Amount,@From,@To,@CurrencyId,@CreatedBy)

END