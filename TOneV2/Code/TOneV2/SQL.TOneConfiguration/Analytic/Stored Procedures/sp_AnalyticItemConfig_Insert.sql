CREATE PROCEDURE [Analytic].[sp_AnalyticItemConfig_Insert]
		@id uniqueidentifier,
	@Name nvarchar(255),
	@Title nvarchar(255),
	@TableId int,
	@ItemType int,
	@Config nvarchar(MAX)

AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM Analytic.AnalyticItemConfig WHERE Name = @Name and ItemType = @ItemType and TableId = @TableId)
	BEGIN
		INSERT INTO Analytic.AnalyticItemConfig(ID,TableId, ItemType,Name,Title,Config)
		VALUES (@id,@TableId,@ItemType,@Name,@Title,@Config)

	END
END