


/*

 */
CREATE PROCEDURE [dbo].[bp_MissMappedCdrsCarrier](@Carrier varchar(50),@From datetime, @Till datetime, @Top int = 100, @MinDuration numeric(13,5) = 0)
AS
BEGIN
	  SET @From=     CAST(
     (
     STR( YEAR( @From ) ) + '-' +
     STR( MONTH( @From ) ) + '-' +
     STR( DAY( @From ) ) 
      )
     AS DATETIME
	)
	
	SET @Till= CAST(
     (
     STR( YEAR( @Till ) ) + '-' +
     STR( MONTH(@Till ) ) + '-' +
     STR( DAY( @Till ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)
	SET NOCOUNT ON
	
	SET ROWCOUNT @Top 
	
	SELECT * INTO #tmpCdrs 
		FROM Billing_CDR_Invalid bi WITH(NOLOCK, INDEX(IX_Billing_CDR_Invalid_Attempt)) 
		WHERE
			bi.Attempt BETWEEN @From AND @Till
			AND	bi.DurationInSeconds > @MinDuration
			AND (bi.SupplierID IS NULL OR bi.CustomerID IS NULL)
	
	SELECT c.* FROM CDR c WITH(NOLOCK, INDEX(IX_CDR_AttemptDateTime)), #tmpCdrs bi 
	WHERE 
			c.AttemptDateTime = bi.Attempt
		AND c.IDonSwitch = bi.SwitchCdrID
		AND c.SwitchID = bi.SwitchID
		AND c.TAG = bi.Tag
		AND c.CDPN = bi.CDPN
		AND c.DurationInSeconds = bi.DurationInSeconds
		AND ( (c.IN_CARRIER = '' AND c.OUT_CARRIER = @Carrier) OR (c.OUT_CARRIER = '' AND c.IN_CARRIER = @Carrier))
	
	DROP TABLE #tmpCdrs
	
	RETURN 0
END