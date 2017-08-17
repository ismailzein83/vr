
CREATE PROCEDURE [Retail_BE].[sp_AccountPackageRecurCharge_UpdateToSent]
@EffectiveDate Datetime
AS
BEGIN
	
	Update [Retail_BE].[AccountPackageRecurCharge] set IsSentToAccountBalance = 1 
	where ChargeDay>=@EffectiveDate AND BalanceAccountTypeID is not null AND BalanceAccountID is not null  
END