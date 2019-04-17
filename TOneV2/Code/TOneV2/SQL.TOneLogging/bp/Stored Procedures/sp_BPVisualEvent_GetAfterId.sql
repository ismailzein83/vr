-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPVisualEvent_GetAfterId]
	@GreaterThanID BIGINT,
	@BPInstanceID INT
AS
BEGIN
IF (@GreaterThanID IS NULL)
	BEGIN
	SELECT  [ID]
			,[ProcessInstanceID]
			,[ActivityID]
			,[Title]
			,[EventTypeID]
			,[EventPayload]
			,[CreatedTime]
            INTO #temp_table
			FROM [bp].[BPVisualEvent] WITH(NOLOCK) 
            WHERE ProcessInstanceID  = @BPInstanceID
            
            SELECT * FROM #temp_table
	END
	
	ELSE
	BEGIN
	SELECT  [ID]
			,[ProcessInstanceID]
			,[ActivityID]
			,[Title]
			,[EventTypeID]
			,[EventPayload]
			,[CreatedTime]
            INTO #temp2_table
            FROM [bp].[BPVisualEvent] WITH(NOLOCK) 
			WHERE ProcessInstanceID  = @BPInstanceID 
			AND ID >@GreaterThanID
            ORDER BY ID
            
            SELECT * FROM #temp2_table
	END
END