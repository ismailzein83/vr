create PROCEDURE [common].[sp_VRTempPayload_Insert]
	@VRTempPayloadId uniqueidentifier,
	@Settings nvarchar(max),	
	@CreatedBy int
AS
BEGIN

	INSERT INTO common.VRTempPayload (ID, Settings, CreatedBy)
	VALUES (@VRTempPayloadId, @Settings, @CreatedBy)


END