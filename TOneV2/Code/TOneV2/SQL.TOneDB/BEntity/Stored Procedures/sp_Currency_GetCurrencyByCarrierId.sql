
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Currency_GetCurrencyByCarrierId]
@CarrierAccountId varchar(5)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT cur.CurrencyID,
	cur.Name,
	cur.IsMainCurrency,
	cur.IsVisible,
	cur.LastRate,
	cur.LastUpdated,
	cur.UserID
	FROM [dbo].Currency cur
join carrierprofile cp on cp.CurrencyID = cur.CurrencyId
join carrieraccount ca on ca.profileid = cp.profileid
where ca.carrieraccountid = @CarrierAccountId
END