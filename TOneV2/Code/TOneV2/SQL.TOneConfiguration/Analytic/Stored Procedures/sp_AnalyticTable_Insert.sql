CREATE PROCEDURE [Analytic].[sp_AnalyticTable_Insert]
	@Id uniqueidentifier,
	@Name nvarchar(255),
	@DevProjectId uniqueidentifier,
	@Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM Analytic.AnalyticTable WHERE Name = @Name)
	BEGIN
		INSERT INTO Analytic.AnalyticTable(ID,Name,DevProjectId, Settings)
		VALUES (@Id,@Name,@DevProjectId, @Settings)
	END
END