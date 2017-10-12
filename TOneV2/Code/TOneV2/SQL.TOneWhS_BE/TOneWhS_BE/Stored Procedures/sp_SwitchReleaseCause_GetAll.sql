-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SwitchReleaseCause_GetAll] 
	
AS
BEGIN
	SELECT	ReleaseCode,ID,SwitchID,Settings
	FROM	SwitchReleaseCause with(nolock)
END