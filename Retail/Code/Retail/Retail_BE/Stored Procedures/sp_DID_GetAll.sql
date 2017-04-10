
CREATE PROCEDURE [Retail_BE].[sp_DID_GetAll]
AS
BEGIN
	SELECT ID, Number, Settings, SourceID
	FROM Retail_BE.DID  with(nolock)
END