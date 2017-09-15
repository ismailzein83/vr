CREATE PROCEDURE [Analytic].[sp_AnalyticTable_Update]
	@ID uniqueidentifier,
	@Name nvarchar(255), 
	@Settings nvarchar(MAX)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM Analytic.AnalyticTable WHERE ID != @ID AND Name = @Name)
	BEGIN
		Update Analytic.AnalyticTable
	Set Name = @Name, Settings = @Settings
	Where ID = @ID
	END
END