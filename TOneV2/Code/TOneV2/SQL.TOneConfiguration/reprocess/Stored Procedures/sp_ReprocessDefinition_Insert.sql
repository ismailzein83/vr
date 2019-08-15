﻿CREATE PROCEDURE [reprocess].[sp_ReprocessDefinition_Insert]
	@Id uniqueidentifier ,
	@Name Nvarchar(255),
	@DevProjectId uniqueidentifier,
	@Settings nvarchar(MAX)

AS
BEGIN
IF NOT EXISTS(select 1 from [reprocess].ReprocessDefinition where Name = @Name)
	BEGIN
		Insert into [reprocess].ReprocessDefinition (Id,[Name],[DevProjectId], [Settings])
		values(@Id,@Name,@DevProjectId, @Settings)
	END
END