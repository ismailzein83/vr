CREATE PROCEDURE [Analytic].[sp_AnalyticItemConfig_Insert]
	@Name nvarchar(255),
	@Title nvarchar(255),
	@TableId int,
	@ItemType int,
	@Config nvarchar(MAX), 
	@id INT OUT
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM Analytic.AnalyticItemConfig WHERE Name = @Name and ItemType = @ItemType and TableId = @TableId)
	BEGIN
		INSERT INTO Analytic.AnalyticItemConfig(TableId, ItemType,Name,Title,Config)
		VALUES (@TableId,@ItemType,@Name,@Title,@Config)

		SET @id = SCOPE_IDENTITY()
	END
END