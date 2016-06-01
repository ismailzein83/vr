create PROCEDURE Retail.[sp_Package_Update]
	@ID int,
	@Name nvarchar(255), 
	@Description nvarchar(1000),
	@Settings nvarchar(MAX)
AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM Retail.Package WHERE ID != @ID AND Name = @Name)
	BEGIN
		Update Retail.Package
	Set Name = @Name, Settings = @Settings,Description = @Description
	Where ID = @ID
	END
END