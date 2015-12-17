
CREATE PROCEDURE [QM_CLITester].[sp_Profile_Update]
	@ID int,
	@Name nvarchar(255),
	@Settings nvarchar(max)
AS
BEGIN
IF NOT EXISTS(select 1 from QM_CLITester.Profile where Name = @Name and Id!=@ID) 
BEGIN
	Update QM_CLITester.Profile
	Set
	 Name = @Name,
	 Settings = @Settings
	Where ID = @ID
END
END