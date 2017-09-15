CREATE PROCEDURE [Analytic].[sp_AnalyticTable_Insert]
	@Id uniqueidentifier,
	@Name nvarchar(255),
	@Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM Analytic.AnalyticTable WHERE Name = @Name)
	BEGIN
		INSERT INTO Analytic.AnalyticTable(Id,Name, Settings)
		VALUES (@Id,@Name, @Settings)
	END
END