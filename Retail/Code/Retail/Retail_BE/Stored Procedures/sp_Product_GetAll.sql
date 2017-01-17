
Create PROCEDURE [Retail_BE].[sp_Product_GetAll]
AS
BEGIN
	SELECT ID, Name, Settings
	FROM Retail_BE.Product  with(nolock)
END