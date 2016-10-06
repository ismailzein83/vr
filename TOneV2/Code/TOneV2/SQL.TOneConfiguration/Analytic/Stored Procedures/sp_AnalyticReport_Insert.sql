CREATE PROCEDURE [Analytic].[sp_AnalyticReport_Insert]
		@id uniqueidentifier ,
	@Name nvarchar(255),
	@UserID int,
	@AccessType int,
	@Settings nvarchar(MAX) 

AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM Analytic.AnalyticReport WHERE Name = @Name and UserID = @UserID)
	BEGIN
		INSERT INTO Analytic.AnalyticReport(ID,UserID,Name,AccessType, Settings)
		VALUES (@id,@UserID,@Name,@AccessType, @Settings)

	END
END