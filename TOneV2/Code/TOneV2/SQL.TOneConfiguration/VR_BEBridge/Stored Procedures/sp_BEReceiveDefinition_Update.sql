﻿
CREATE PROCEDURE [VR_BEBridge].[sp_BEReceiveDefinition_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@DevProjectId  uniqueidentifier,
	@Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM VR_BEBridge.BEReceiveDefinition WHERE ID != @ID and Name = @Name )
	BEGIN
		update VR_BEBridge.BEReceiveDefinition
		set  Name = @Name,DevProjectID=@DevProjectId,Settings= @Settings, LastModifiedTime = GETDATE()
		where  ID = @ID
	END

END