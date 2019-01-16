CREATE PROCEDURE [common].[sp_Setting_Update]
	@ID uniqueidentifier,
	@Name nvarchar(255),
	@Data nvarchar(max)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM common.Setting WHERE ID != @ID and Name = @Name)
	BEGIN
		Update common.Setting
		Set [Name] = @Name,
		[Data] = @Data
		Where ID = @ID
	END
END