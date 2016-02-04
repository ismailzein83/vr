﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [genericdata].[sp_DataRecordType_Update]
	@ID INT,
	@Name nvarchar(255),
	@ParentId INT,
	@Fields VARCHAR(MAX)
	
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM genericdata.[DataRecordType] WHERE ID != @ID AND Name = @Name)
	BEGIN
		Update genericdata.[DataRecordType]
		Set Name = @Name, ParentID = @ParentId , Fields = @Fields
		Where ID = @ID
	END
END