CREATE PROCEDURE [Analytic].[sp_AnalyticReport_Update]
	@ID uniqueidentifier,
	@Name nvarchar(255), 
	@UserID int,
	@AccessType int,
	@Settings nvarchar(MAX)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM Analytic.AnalyticReport WHERE ID != @ID AND Name = @Name and UserID = @UserID)
	BEGIN
		Update Analytic.AnalyticReport
	Set Name = @Name, Settings = @Settings,AccessType = @AccessType
	Where ID = @ID
	END
END