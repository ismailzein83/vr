CREATE PROCEDURE [QM_BE].[sp_Zone_UpdateFromSource]
	@ID int,
	@Name nvarchar(255)
AS
BEGIN

	Update QM_BE.Zone
	Set
	 Name = @Name
	Where ID = @ID

END