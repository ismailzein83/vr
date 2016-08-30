CREATE PROCEDURE [Analytic].[sp_AnalyticReport_Insert]
	@Name nvarchar(255),
	@UserID int,
	@AccessType int,
	@Settings nvarchar(MAX), 
	@id INT OUT
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM Analytic.AnalyticReport WHERE Name = @Name and UserID = @UserID)
	BEGIN
		INSERT INTO Analytic.AnalyticReport(UserID,Name,AccessType, Settings)
		VALUES (@UserID,@Name,@AccessType, @Settings)

		SET @id = SCOPE_IDENTITY()
	END
END