CREATE PROCEDURE [dbo].[bp_UpdateOperationQueue]
(
	@From datetime,
	@Till datetime 
)
AS
BEGIN
	DECLARE @OperationQueue TABLE ([Type] char(1) NOT NULL, [ID] bigint NOT NULL, Operation char(1) NOT NULL)
	DECLARE @WorkingType char(1)
	DECLARE @EndFlag char(1)
	DECLARE @BeginFlag char(1) 
	SET @BeginFlag = 'B'
	SET @EndFlag = 'E'	
		
	-- Process ToD
	SET @WorkingType = 'T'
	INSERT INTO @OperationQueue 
		SELECT @WorkingType, T.ToDConsiderationID, @BeginFlag 
			FROM ToDConsideration T WITH(NOLOCK) 
			WHERE 
					(T.BeginEffectiveDate BETWEEN @From AND @Till) 
				AND 'Y' = dbo.IsToDActive(T.HolidayDate, T.WeekDay, T.BeginTime, T.EndTime, @From)
	INSERT INTO @OperationQueue 
		SELECT @WorkingType, T.ToDConsiderationID, @EndFlag 
			FROM ToDConsideration T WITH(NOLOCK) 
			WHERE 
					(T.EndEffectiveDate BETWEEN @From AND @Till) 
				OR (					
					(T.BeginEffectiveDate BETWEEN @From AND @Till) 
						AND 'N' = dbo.IsToDActive(T.HolidayDate, T.WeekDay, T.BeginTime, T.EndTime, @From)
					)
	
	-- Process Special Requests
	SET @WorkingType = 'S'
	INSERT INTO @OperationQueue 
		SELECT @WorkingType, S.SpecialRequestID, @BeginFlag 
			FROM SpecialRequest S WITH(NOLOCK)
			WHERE S.BeginEffectiveDate BETWEEN @From AND @Till
	INSERT INTO @OperationQueue 
		SELECT @WorkingType, S.SpecialRequestID, @EndFlag 
			FROM SpecialRequest S WITH(NOLOCK) 
			WHERE S.EndEffectiveDate BETWEEN @From AND @Till
	
	-- Process Route Blocks
	SET @WorkingType = 'B'
	INSERT INTO @OperationQueue 
		SELECT @WorkingType, B.RouteBlockID, @BeginFlag 
			FROM RouteBlock B WITH(NOLOCK) 
			WHERE B.BeginEffectiveDate BETWEEN @From AND @Till
	INSERT INTO @OperationQueue 
		SELECT @WorkingType, B.RouteBlockID, @EndFlag 
			FROM RouteBlock B WITH(NOLOCK) 
			WHERE B.EndEffectiveDate BETWEEN @From AND @Till

	-- Update OperationQueue
	INSERT INTO OperationQueue 
		(Type, ID, Operation, Created)
	SELECT 
		Type, ID, Operation, @From 
		FROM @OperationQueue
	
	-- Clean Up OperationQueue
	DELETE FROM @OperationQueue
	INSERT INTO @OperationQueue(Type, ID, Operation)
		SELECT Type, ID, MIN(Operation) 
			FROM OperationQueue WITH(NOLOCK)
			GROUP BY Type, ID
			HAVING Count(*) > 1  
	 
	-- DELETE Duplicate Entries
	DELETE FROM OperationQueue  
		WHERE EXISTS(SELECT * FROM @OperationQueue O WHERE O.Type = OperationQueue.Type AND O.ID = OperationQueue.ID)	

END