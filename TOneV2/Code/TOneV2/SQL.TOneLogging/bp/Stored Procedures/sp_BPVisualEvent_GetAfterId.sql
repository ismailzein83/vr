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

	SELECT [ID]
			,[ProcessInstanceID]
			,[ActivityID]
			,[Title]
			,[EventTypeID]
			,[EventPayload]
			,[CreatedTime]
			FROM [bp].[BPVisualEvent] WITH(NOLOCK) 
            WHERE ProcessInstanceID  = @BPInstanceID
            and (@GreaterThanID IS NULL OR ID >@GreaterThanID)
			ORDER BY ID
END