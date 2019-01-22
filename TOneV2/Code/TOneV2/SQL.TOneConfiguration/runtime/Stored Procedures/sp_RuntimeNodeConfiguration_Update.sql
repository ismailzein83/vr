﻿CREATE PROCEDURE [runtime].[sp_RuntimeNodeConfiguration_Update]
	@ID uniqueidentifier,
	@Name nvarchar(255),
	@Setting nvarchar(max)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM runtime.RuntimeNodeConfiguration WHERE ID != @ID AND Name = @Name)
	BEGIN
		Update runtime.[RuntimeNodeConfiguration]
	Set Name = @Name,
	Settings = @Setting,
	LastModifiedTime= GETDATE()
	Where ID = @ID
	END
END