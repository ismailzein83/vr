create PROCEDURE [common].[sp_VRTempPayload_Delete]
	@VRTempPayloadId uniqueidentifier
AS
BEGIN
	delete from common.VRTempPayload
	WHERE ID = @VRTempPayloadId
END