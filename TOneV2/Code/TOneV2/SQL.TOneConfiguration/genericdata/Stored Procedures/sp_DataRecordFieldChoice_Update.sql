﻿CREATE PROCEDURE [genericdata].[sp_DataRecordFieldChoice_Update]
	@ID uniqueidentifier,
	@Name nvarchar(255), 
	@DevProjectId uniqueidentifier,
	@Settings nvarchar(MAX)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM genericdata.DataRecordFieldChoice WHERE ID != @ID AND Name = @Name)
	BEGIN
		Update genericdata.DataRecordFieldChoice
	Set Name = @Name,DevProjectId=@DevProjectId, Settings = @Settings, LastModifiedTime = GETDATE()
	Where ID = @ID
	END
END