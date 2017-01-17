
CREATE PROCEDURE [Retail_BE].[sp_DID_GetAll]
AS
BEGIN
	SELECT ID, Number, Settings
	FROM Retail_BE.DID  with(nolock)
END