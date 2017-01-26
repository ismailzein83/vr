
Create PROCEDURE [Retail_BE].[sp_ProductFamily_Update]
	@ID int,
	@Name NVARCHAR(255),
	@Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM Retail_BE.ProductFamily WHERE ID != @ID and Name = @Name)
	BEGIN
		update Retail_BE.ProductFamily
		set  Name = @Name, Settings= @Settings
		where  ID = @ID
	END
END