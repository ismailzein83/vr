CREATE PROCEDURE [Retail_BE].[sp_CreditClass_GetAll]
AS
BEGIN
	SELECT	ID, Name, Settings
	FROM	[Retail_BE].[CreditClass]  with(nolock)
END