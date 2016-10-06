CREATE PROCEDURE [Analytic].[sp_AnalyticItemConfig_Update]
	@ID uniqueidentifier,
	@Name nvarchar(255),
	@Title nvarchar(255),
	@TableId int,
	@ItemType int,
	@Config nvarchar(MAX)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM Analytic.AnalyticItemConfig WHERE ID != @ID AND Name = @Name and ItemType = @ItemType and TableId = @TableId)
	BEGIN
		Update Analytic.AnalyticItemConfig
	Set Name = @Name, Title = @Title , ItemType = @ItemType, Config = @Config,TableId= @TableId
	Where ID = @ID
	END
END