CREATE PROCEDURE [common].[sp_VRTimeZone_Insert]
	@Name nvarchar(255),
	@Settings nvarchar(max),
	@CreatedBy int,
	@LastModifiedBy int,
	@Id int out
	
AS

BEGIN
SET @Id =0;
IF NOT EXISTS(SELECT 1 FROM [common].[VRTimeZone] WHERE Name = @Name)
	BEGIN
		INSERT INTO [common].[VRTimeZone](Name, Settings ,CreatedTime, CreatedBy, LastModifiedBy, LastModifiedTime)
		VALUES ( @Name , @Settings , GETDATE(), @CreatedBy, @LastModifiedBy, GETDATE())
		Set @Id = @@IDENTITY
	END
END