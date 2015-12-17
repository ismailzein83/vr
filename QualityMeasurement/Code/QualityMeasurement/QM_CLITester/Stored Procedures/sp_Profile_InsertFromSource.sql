
CREATE PROCEDURE [QM_CLITester].[sp_Profile_InsertFromSource]
	@ID int,
	@Name nvarchar(255),
	@SourceProfileID varchar(255),
	@Settings nvarchar(max)
AS

BEGIN
	Insert into QM_CLITester.[Profile]([ID],[Name],[SourceProfileID], [Settings])
	Values(@ID,@Name, @SourceProfileID, @Settings)
END