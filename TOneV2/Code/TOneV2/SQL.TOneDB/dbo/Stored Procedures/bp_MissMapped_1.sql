
/*
 * Get CDRs that were considered invalid with unknown Customer or Supplier 
 */
CREATE PROCEDURE [dbo].[bp_MissMapped](@From DATETIME, @Till DATETIME, @Top int = 50, @MinDuration numeric(13,5) = 0)
WITH RECOMPILE
AS
BEGIN
	


	SET NOCOUNT ON;
	
	--SET ROWCOUNT @Top 
With InvalidCustomerCDR As(
	SELECT bi.* 
		FROM Billing_CDR_Invalid bi WITH(NOLOCK,INDEX(IX_Billing_CDR_Invalid_Attempt)) 
	WHERE  bi.Attempt >=@From AND bi.Attempt <@Till
			AND bi.CustomerID IS NULL
			AND	bi.DurationInSeconds > @MinDuration
)
,
InvalidSupplierCDR As (
	SELECT bi.* 
		FROM Billing_CDR_Invalid bi WITH(NOLOCK,INDEX(IX_Billing_CDR_Invalid_Attempt)) 
		JOIN CarrierAccount cac ON (cac.CarrierAccountID = bi.CustomerID AND cac.RepresentsASwitch <> 'Y')
	WHERE  bi.Attempt >=@From AND bi.Attempt <@Till
			AND bi.SupplierID IS NULL
			AND	bi.DurationInSeconds > @MinDuration
)
,CDRCTE As (
	SELECT AttemptDateTime,IDonSwitch,SwitchID,TAG,DurationInSeconds,CDPN,IN_CARRIER,OUT_CARRIER
	FROM CDR c WITH(NOLOCK,index=IX_CDR_AttemptDateTime)
	WHERE c.AttemptDateTime >=@From AND c.AttemptDateTime <@Till
)	

,MissingCustomer As (
	SELECT Carrier = c.IN_CARRIER,[Type] = 'Missing Customer' 
	FROM CDRCTE c , InvalidCustomerCDR icc 
	WHERE 
		--c.AttemptDateTime >=@From AND c.AttemptDateTime <@Till AND
			c.AttemptDateTime = icc.Attempt
		AND c.IDonSwitch = icc.SwitchCdrID
		AND c.SwitchID = icc.SwitchID
		AND c.TAG = icc.Tag
		AND c.CDPN = icc.CDPN
		AND c.DurationInSeconds = icc.DurationInSeconds
	GROUP BY c.IN_CARRIER
)	
,MissingSupplier As (	
	SELECT Carrier = c.OUT_CARRIER,[Type] = 'Missing Supplier' 
	FROM CDRCTE c , InvalidSupplierCDR isc 
	WHERE 
		--c.AttemptDateTime >=@From AND c.AttemptDateTime <@Till AND	
		c.AttemptDateTime = isc.Attempt
		AND c.IDonSwitch = isc.SwitchCdrID
		AND c.SwitchID = isc.SwitchID
		AND c.TAG = isc.Tag
		AND c.CDPN LIKE ('%' + isc.CDPN)
		AND c.DurationInSeconds = isc.DurationInSeconds
	GROUP BY c.OUT_CARRIER
)	

Select * from MissingCustomer 
	UNION 
Select * from MissingSupplier 

	--DROP TABLE #tmpSupplier
	--DROP TABLE #tmpCustomer
	
	RETURN 
END