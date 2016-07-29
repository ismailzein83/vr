Create PROCEDURE [Retail_BE].[sp_CreditClass_Update]
	@ID int,
	@Name Nvarchar(255),
	@Settings nvarchar(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM [Retail_BE].CreditClass WHERE ID != @ID AND Name = @Name)
	BEGIN
		UPDATE [Retail_BE].CreditClass
		SET Name = @Name,
			[Settings] = @Settings
		WHERE ID = @ID
	END
END