
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Currency_GetCurrencyIdByCarrierId]
@CarrierAccountId varchar(5)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
SELECT currencyid FROM CarrierProfile cp
join CarrierAccount ca on ca.profileid = cp.profileid
where ca.carrieraccountid = @CarrierAccountId
	
END