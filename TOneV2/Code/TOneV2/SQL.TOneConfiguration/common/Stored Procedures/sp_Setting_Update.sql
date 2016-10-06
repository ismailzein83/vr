CREATE PROCEDURE [common].[sp_Setting_Update]
	@ID uniqueidentifier,
	@Name nvarchar(255),
	@Data nvarchar(max)
AS
BEGIN
IF EXISTS(SELECT 1 FROM common.Setting WHERE ID = @ID )
	BEGIN
		Update common.Setting
		Set [Name] = @Name,
		[Data] = @Data
		Where ID = @ID
	END
END