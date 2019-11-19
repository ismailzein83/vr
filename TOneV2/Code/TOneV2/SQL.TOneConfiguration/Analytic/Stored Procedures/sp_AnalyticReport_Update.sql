CREATE PROCEDURE [Analytic].[sp_AnalyticReport_Update]
	@ID uniqueidentifier,
	@Name nvarchar(255), 
	@Title nvarchar(255),
	@UserID int,
	@AccessType int,
	@DevProjectId uniqueidentifier,
	@Settings nvarchar(MAX)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM Analytic.AnalyticReport WHERE ID != @ID AND Name = @Name and UserID = @UserID)
	BEGIN
		Update Analytic.AnalyticReport
	Set Name = @Name,Title = @Title, Settings = @Settings,AccessType = @AccessType,DevProjectId=@DevProjectId, LastModifiedTime = getdate()
	Where ID = @ID
	END
END