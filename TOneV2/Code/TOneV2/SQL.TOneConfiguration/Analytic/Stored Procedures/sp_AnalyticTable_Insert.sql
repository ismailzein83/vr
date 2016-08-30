CREATE PROCEDURE [Analytic].[sp_AnalyticTable_Insert]
	@Name nvarchar(255),
	@Settings nvarchar(MAX), 
	@id INT OUT
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM Analytic.AnalyticTable WHERE Name = @Name)
	BEGIN
		INSERT INTO Analytic.AnalyticTable(Name, Settings)
		VALUES (@Name, @Settings)

		SET @id = SCOPE_IDENTITY()
	END
END