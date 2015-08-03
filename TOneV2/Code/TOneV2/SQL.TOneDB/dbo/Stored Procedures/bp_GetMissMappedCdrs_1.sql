


/*
 * Get CDRs that were considered invalid with unknown Customer or Supplier 
 */
CREATE PROCEDURE [dbo].[bp_GetMissMappedCdrs](@From datetime, @Till datetime, @Top int = 100, @MinDuration numeric(13,5) = 0)
With recompile
AS

BEGIN

	SET NOCOUNT ON;
	
	--SET ROWCOUNT @Top 
	With tmpCdrs As (
	SELECT * 
		FROM Billing_CDR_Invalid bi WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt)) 
	JOIN CarrierAccount cac ON bi.CustomerID = cac.CarrierAccountID 
	WHERE  bi.Attempt >=@From AND bi.Attempt <@Till
			AND ((bi.SupplierID IS NULL) OR (bi.CustomerID IS NULL))
			AND	bi.DurationInSeconds > @MinDuration
	        AND cac.RepresentsASwitch <> 'Y'
	)
	SELECT c.* FROM CDR c WITH(NOLOCK, INDEX(IX_CDR_AttemptDateTime)), tmpCdrs bi 
	WHERE c.AttemptDateTime >=@From AND c.AttemptDateTime <@Till
		AND c.AttemptDateTime = bi.Attempt
		AND c.IDonSwitch = bi.SwitchCdrID
		AND c.SwitchID = bi.SwitchID
		AND c.TAG = bi.Tag
		AND c.CDPN LIKE ('%' + bi.CDPN)
		AND c.DurationInSeconds = bi.DurationInSeconds
		
	
--	DROP TABLE #tmpCdrs
	
	RETURN 0
END