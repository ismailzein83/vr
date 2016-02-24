

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [CP_SupPriceList].[sp_Customer_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	c.ID,
			c.Name,
			c.Settings
		
	FROM	[CP_SupPriceList].[Customer]  as c WITH(NOLOCK) 
END