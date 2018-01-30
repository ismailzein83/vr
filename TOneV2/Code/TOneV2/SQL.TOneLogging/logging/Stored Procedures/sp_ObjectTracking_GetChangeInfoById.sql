-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [logging].sp_ObjectTracking_GetChangeInfoById 
	@objectTrackingId int
AS
BEGIN
	SELECT	
		ac.ChangeInfo
		
FROM	[logging].[ObjectTracking]  ac WITH(NOLOCK)
WHERE	ac.ID=@objectTrackingId
		
END