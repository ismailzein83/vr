-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_AccountUsage_TryAddAndGet]
	@AccountTypeID uniqueidentifier,
	@TransactionTypeID uniqueidentifier,
	@AccountID varchar(50),
	@PeriodStart datetime,
	@PeriodEnd datetime,
	@CurrencyId int,
	@UsageBalance decimal(20,6)
AS
BEGIN

	declare @ID bigint;
	declare @IsOverridden bit; set @IsOverridden = null;
	declare @OverriddenAmount decimal(20, 6); set @OverriddenAmount = null;
	
	if exists (select top 1 null from VR_AccountBalance.AccountUsageOverride where AccountTypeID = @AccountTypeID and AccountID = @AccountID and TransactionTypeID = @TransactionTypeID and PeriodStart <= @PeriodStart and PeriodEnd >= @PeriodEnd)
	begin
		set @IsOverridden = 1;
		set @OverriddenAmount = 0;
	end

	select @ID = ID from [VR_AccountBalance].AccountUsage where AccountID = @AccountID and AccountTypeID = @AccountTypeID and TransactionTypeID = @TransactionTypeID and PeriodStart = @PeriodStart
	if (@ID is null)
	begin
		insert into [VR_AccountBalance].AccountUsage (AccountTypeID, TransactionTypeID, AccountID, CurrencyId, PeriodStart, PeriodEnd, UsageBalance, IsOverridden, OverriddenAmount)
		values (@AccountTypeID, @TransactionTypeID, @AccountID, @CurrencyId, @PeriodStart, @PeriodEnd, @UsageBalance, @IsOverridden, @OverriddenAmount)
		set @ID = scope_identity();
	end

	select @ID as ID, @AccountID as AccountID, @TransactionTypeID as TransactionTypeID, @IsOverridden as IsOverridden
END