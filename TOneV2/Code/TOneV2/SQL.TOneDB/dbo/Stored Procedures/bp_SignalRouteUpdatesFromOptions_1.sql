CREATE PROCEDURE [dbo].[bp_SignalRouteUpdatesFromOptions](@UpdateStamp datetime, @UpdateType varchar(20))

AS
	UPDATE [Route]
		SET [Route].Updated = [RouteOption].Updated
			,[Route].IsToDAffected =			CASE WHEN @UpdateType LIKE 'TOD' THEN 1 ELSE [Route].IsToDAffected END
			,[Route].IsSpecialRequestAffected = CASE WHEN @UpdateType LIKE 'SpecialRequests' THEN 1 ELSE [Route].IsSpecialRequestAffected END
			,[Route].IsBlockAffected =			CASE WHEN @UpdateType LIKE 'RouteBlocks' THEN 1 ELSE [Route].IsBlockAffected END
	FROM [Route] WITH(NOLOCK), [RouteOption] WITH(NOLOCK)
		WHERE [Route].RouteID=[RouteOption].RouteID
		AND [RouteOption].Updated >= @UpdateStamp
		AND [Route].Updated < [RouteOption].Updated