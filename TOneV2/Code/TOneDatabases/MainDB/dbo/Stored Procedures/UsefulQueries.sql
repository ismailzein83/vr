CREATE PROCEDURE [dbo].[UsefulQueries]
	with Recompile
AS
BEGIN

	RETURN 0

	/*
	* Convert Special Requests to Route Overrides 
	*/	
	DELETE RouteOverride 
		FROM RouteOverride, SpecialRequest sr 
		WHERE RouteOverride.CustomerID = sr.CustomerID AND RouteOverride.Code = sr.Code  

	INSERT INTO RouteOverride
	(
		CustomerID,
		Code,
		IncludeSubCodes,
		OurZoneID,
		RouteOptions,
		BlockedSuppliers,
		BeginEffectiveDate
	)
	SELECT sr.CustomerID, sr.Code, 'N', 0, '', NULL, MIN(sr.BeginEffectiveDate) 
		FROM SpecialRequest sr
		GROUP BY sr.CustomerID, sr.Code

	DECLARE @Code VARCHAR(30)
	DECLARE @CustomerID VARCHAR(30)
	DECLARE @SupplierID VARCHAR(30)

	DECLARE curSpecialRequests CURSOR LOCAL FAST_FORWARD FOR 
		SELECT sr.CustomerID, sr.Code, sr.SupplierID
			FROM SpecialRequest sr (NOLOCK)
		ORDER BY sr.CustomerID, sr.Code, sr.Priority DESC
	OPEN curSpecialRequests
	FETCH NEXT FROM curSpecialRequests INTO @CustomerID, @Code, @SupplierID 	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		UPDATE RouteOverride
			SET RouteOptions = CASE WHEN RouteOptions = '' THEN @SupplierID ELSE RouteOptions + '|' + @SupplierID END
			WHERE CustomerID = @CustomerID AND Code = @Code
			AND LEN(RouteOptions) < 90
		FETCH NEXT FROM curSpecialRequests INTO @CustomerID, @Code, @SupplierID 	
	END
	CLOSE curSpecialRequests
	DEALLOCATE curSpecialRequests	

	SELECT * FROM RouteOverride ro

	SELECT * FROM SpecialRequest sr WHERE sr.SupplierID = 'C015' AND sr.CustomerID = 'C042' ORDER BY sr.Code

END