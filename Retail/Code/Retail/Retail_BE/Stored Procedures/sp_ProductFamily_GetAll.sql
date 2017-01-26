
Create PROCEDURE [Retail_BE].[sp_ProductFamily_GetAll]
AS
BEGIN
	SELECT ID, Name, Settings
	FROM Retail_BE.ProductFamily  with(nolock)
END