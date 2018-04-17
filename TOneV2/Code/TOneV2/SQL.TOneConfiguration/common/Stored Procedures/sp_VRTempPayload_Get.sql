-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [common].[sp_VRTempPayload_Get]
AS
BEGIN

	SELECT	ID, Settings,CreatedBy,CreatedTime
	FROM	[common].VRTempPayload   WITH(NOLOCK) 
END