
CREATE PROCEDURE [Retail_BE].[sp_DID_Update]
	@ID int,
	@Settings nvarchar(MAX)
AS
BEGIN
	update Retail_BE.DID 
	set Settings= @Settings
	where ID = @ID
END