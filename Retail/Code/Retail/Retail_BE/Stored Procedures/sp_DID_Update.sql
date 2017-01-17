
CREATE PROCEDURE [Retail_BE].[sp_DID_Update]
	@ID int,
	@Number NVARCHAR(50),
	@Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM Retail_BE.DID WHERE ID != @ID and Number = @Number)
	BEGIN
		update Retail_BE.DID 
		set  Number = @Number, Settings= @Settings
		where  ID = @ID
	END
END