-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SwitchReleaseCause_GetAll] 
	
AS
BEGIN
	SELECT	ReleaseCode,ID,SwitchID,Settings,SourceID, CreatedTime, CreatedBy, LastModifiedBy, LastModifiedTime
	FROM	[TOneWhS_BE].SwitchReleaseCause with(nolock)
END