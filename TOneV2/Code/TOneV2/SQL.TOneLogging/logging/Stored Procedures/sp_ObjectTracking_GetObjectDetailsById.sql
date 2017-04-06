﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [logging].[sp_ObjectTracking_GetObjectDetailsById] 
	@objectTrackingId int
AS
BEGIN
	SELECT	
		ac.ObjectDetails
		
FROM	[logging].[ObjectTracking]  ac WITH(NOLOCK)
WHERE	ac.ID=@objectTrackingId
		
END