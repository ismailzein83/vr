
CREATE PROCEDURE [QM_CLITester].[sp_Profile_UpdateFromSource]
	@ID int,
	@Name nvarchar(255),
	@Settings nvarchar(max)
AS
BEGIN

	Update [QM_CLITester].[Profile]
	Set
	 Name = @Name, 
	 Settings = @Settings
	Where ID = @ID

END