
CREATE PROCEDURE [Retail_BE].[sp_RecurChargeBalanceUpdateSummary_Insert]
@ChargeDay DateTime,
@Data varchar(MAX)
AS
BEGIN
	INSERT [Retail_BE].[RecurChargeBalanceUpdateSummary] ([ChargeDay], [Data]) 
	SELECT @ChargeDay, @Data
END