CREATE PROCEDURE [dbo].[bp_GetHourlyChangeFlags]
(
	@When datetime = getdate,
	@RateChanges char(1) output,
	@SpecialRequestChanges char(1) output,
	@RouteBlockChanges char(1) output	
)
AS
BEGIN
	SET NOCOUNT ON
	
	-- Declare and Initialize Hour Range
	DECLARE @from datetime
	DECLARE @till datetime	
	SELECT @from = cast((CONVERT(varchar(14), @When, 121)+'00:00.000') AS datetime)
	SELECT @till = dateadd(hour, 1, @from)
	
	SET @RateChanges = 'N'
	SET @SpecialRequestChanges = 'N'
	SET @RouteBlockChanges = 'N'
	
	-- Rates?
	IF EXISTS(
			SELECT * FROM Rate R 
				WHERE 
					(R.BeginEffectiveDate >= @from AND R.BeginEffectiveDate < @till)
					OR
					(R.EndEffectiveDate >= @from AND R.EndEffectiveDate < @till)
		)
	BEGIN
		SELECT @RateChanges = 'Y'
	END

	-- Special Requests
	IF EXISTS(
			SELECT * FROM SpecialRequest SR 
				WHERE 
					(SR.BeginEffectiveDate >= @from AND SR.BeginEffectiveDate < @till)
					OR
					(SR.EndEffectiveDate >= @from AND SR.EndEffectiveDate < @till)
		)
	BEGIN
		SELECT @SpecialRequestChanges = 'Y'
	END
	
	-- Route Blocks
	IF EXISTS(
			SELECT * FROM RouteBlock RB 
				WHERE 
					(RB.BeginEffectiveDate >= @from AND RB.BeginEffectiveDate < @till)
					OR
					(RB.EndEffectiveDate >= @from AND RB.EndEffectiveDate < @till)
		)
	BEGIN
		SELECT @RouteBlockChanges = 'Y'
	END
		
END