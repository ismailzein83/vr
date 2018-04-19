-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_VRTempPayload_Get]
	@VRTempPayloadId uniqueidentifier,
	@DeleteBeforeDate datetime
AS
BEGIN

	SELECT	ID, Settings,CreatedBy,CreatedTime
	FROM	[common].VRTempPayload   WITH(NOLOCK)
	WHERE ID = @VRTempPayloadId 

	DELETE FROM	[common].VRTempPayload  WHERE CreatedTime < @DeleteBeforeDate

END