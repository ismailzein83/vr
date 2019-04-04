CREATE PROCEDURE [common].[sp_Setting_Update]
	@ID uniqueidentifier,
	@Data nvarchar(max)
AS
BEGIN
	Update common.Setting
	Set [Data] = @Data
	Where ID = @ID
END