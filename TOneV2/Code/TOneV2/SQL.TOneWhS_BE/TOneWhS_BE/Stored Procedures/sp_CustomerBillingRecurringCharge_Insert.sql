
CREATE PROCEDURE[TOneWhS_BE].[sp_CustomerBillingRecurringCharge_Insert]

@FinancialAccountId int,
@RecurringChargeId bigint,
@InvoiceId bigint,
@Amount decimal(22,6),
@From   datetime,
@To datetime,
@CurrencyId int,
@CreatedBy int,
@VAT decimal(22,6)

AS
BEGIN




	SET NOCOUNT ON;
insert into [TOneWhS_BE].[CustomerBillingRecurringCharge](FinancialAccountId,RecurringChargeId,InvoiceId,Amount,[From],[To],CurrencyId,CreatedBy,VAT) values(@FinancialAccountId,@RecurringChargeId,@InvoiceId,@Amount,@From,@To,@CurrencyId,@CreatedBy,@VAT)

END