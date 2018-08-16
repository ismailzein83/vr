﻿CREATE PROCEDURE [bp].[sp_BPTimeSubscription_GetDue]
AS
BEGIN
	SELECT [ID],[ProcessInstanceID],[Bookmark],[DueTime],[Payload]
	FROM [bp].[BPTimeSubscription] WITH(NOLOCK)
	WHERE DueTime <=GETDATE()
END