﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [logging].[sp_ObjectTracking_GetFiltered]
@LoggableEntityId uniqueidentifier ,
@ObjectId varchar(255)
AS
BEGIN

SELECT	ac.ID,
		ac.UserID,
		ac.ActionID,
		ac.LogTime,
		CASE WHEN ObjectDetails IS NULL THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END as HasDetail
FROM	[logging].[ObjectTracking]  ac WITH(NOLOCK)
WHERE	ac.LoggableEntityID=@LoggableEntityId
		AND ac.ObjectID= @ObjectId
END