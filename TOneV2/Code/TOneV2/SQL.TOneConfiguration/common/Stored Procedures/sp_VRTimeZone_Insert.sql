CREATE PROCEDURE [common].[sp_VRTimeZone_Insert]
	@Name nvarchar(255),
	@Settings nvarchar(max),
	@Id int out
	
AS

BEGIN
SET @Id =0;
IF NOT EXISTS(SELECT 1 FROM [common].[VRTimeZone] WHERE Name = @Name)
	BEGIN
		INSERT INTO [common].[VRTimeZone](Name, Settings ,CreatedTime )
		VALUES ( @Name , @Settings , GETDATE())
		Set @Id = @@IDENTITY
	END
END