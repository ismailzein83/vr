
-- =======================================================================
-- Author:		Fadi Chamieh
-- Create date: 18/12/2007
-- Description:	Reset Routes / Route Options to defaults before an update
-- =======================================================================
CREATE PROCEDURE [dbo].[bp_ResetRoutes](@UpdateType varchar(10))	

AS
BEGIN
	
	DECLARE @State_Blocked tinyint
	DECLARE @State_Enabled tinyint
	SET @State_Blocked = 0
	SET @State_Enabled = 1

	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;

	-- Get the Min Update
	DECLARE @RoutesCreation datetime 
	SELECT @RoutesCreation = MIN(Updated) FROM [Route]
	
	DECLARE @UpdateStamp datetime
	SELECT @UpdateStamp = getdate()
	
	-- Reset Route Options
	UPDATE [RouteOption]		
		SET
			SupplierActiveRate = CASE WHEN @UpdateType = 'TOD' THEN SupplierActiveRate ELSE SupplierActiveRate END
			, Priority = CASE WHEN @UpdateType = 'SpecialRequests' THEN 0 ELSE Priority END
			, NumberOfTries = CASE WHEN @UpdateType = 'SpecialRequests' THEN 1 ELSE NumberOfTries END
			, State = CASE WHEN @UpdateType = 'RouteBlocks' THEN @State_Enabled ELSE RouteOption.State END
			, Updated = @UpdateStamp
		FROM [RouteOption], [Route]
		WHERE
				[RouteOption].RouteID = [Route].RouteID
			AND [Route].Updated > @RoutesCreation
			AND	(
					(@UpdateType = 'TOD' AND [Route].IsToDAffected = 1)
					OR
					(@UpdateType = 'SpecialReqests' AND [Route].IsSpecialRequestAffected = 1)
					OR
					(@UpdateType = 'RouteBlocks' AND [Route].IsBlockAffected = 1)
				)

	-- Reset Routes	
	UPDATE [Route]
		SET
			OurActiveRate = CASE WHEN @UpdateType = 'TOD' THEN OurActiveRate ELSE OurActiveRate END
			,State = CASE WHEN @UpdateType = 'RouteBlocks' THEN @State_Enabled ELSE [State] END
			,[Route].IsSpecialRequestAffected = CASE WHEN @UpdateType = 'SpecialReqests' THEN 0 ELSE [Route].IsSpecialRequestAffected END
			,[Route].IsToDAffected = CASE WHEN @UpdateType = 'TOD' THEN 0 ELSE [Route].IsToDAffected END
			,[Route].IsBlockAffected = CASE WHEN @UpdateType = 'RouteBlocks' THEN 0 ELSE [Route].IsBlockAffected END
			,Updated = @UpdateStamp
		WHERE
			Updated > @RoutesCreation
			AND	(
					(@UpdateType = 'TOD' AND [Route].IsToDAffected = 1)
					OR
					(@UpdateType = 'SpecialReqests' AND [Route].IsSpecialRequestAffected = 1)
					OR
					(@UpdateType = 'RouteBlocks' AND [Route].IsBlockAffected = 1)
			)
END