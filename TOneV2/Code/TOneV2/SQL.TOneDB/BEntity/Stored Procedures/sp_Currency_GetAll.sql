-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_Currency_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT cur.CurrencyID,
	cur.Name,
	cur.IsMainCurrency,
	cur.IsVisible,
	cur.LastRate,
	cur.LastUpdated,
	cur.UserID
	FROM [dbo].Currency cur
END