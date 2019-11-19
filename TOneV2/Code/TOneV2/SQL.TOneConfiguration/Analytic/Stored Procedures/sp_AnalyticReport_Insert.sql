CREATE PROCEDURE [Analytic].[sp_AnalyticReport_Insert]
	@id uniqueidentifier ,
	@Name nvarchar(255),
	@Title nvarchar(255),
	@UserID int,
	@AccessType int,
	@DevProjectId uniqueidentifier,
	@Settings nvarchar(MAX) 

AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM Analytic.AnalyticReport WHERE Name = @Name and UserID = @UserID)
	BEGIN
		INSERT INTO Analytic.AnalyticReport(ID,UserID,Name, Title,AccessType,DevProjectId, Settings)
		VALUES (@id,@UserID,@Name, @Title,@AccessType,@DevProjectId, @Settings)

	END
END