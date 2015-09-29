-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Currency_GetVisible]
AS
BEGIN

	SET NOCOUNT ON;
	SELECT	cur.CurrencyID,
			cur.Name,
			cur.IsMainCurrency,
			cur.IsVisible,
			cur.LastRate,
			cur.LastUpdated,
			cur.UserID
	FROM	[dbo].Currency cur
	Where	cur.IsVisible = 'Y'
	
END