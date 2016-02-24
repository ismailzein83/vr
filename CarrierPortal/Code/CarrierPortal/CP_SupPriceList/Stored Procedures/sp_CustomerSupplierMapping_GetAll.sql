


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [CP_SupPriceList].[sp_CustomerSupplierMapping_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT	c.ID,
			c.UserID,
			c.CustomerID,
			c.MappingSettings		
	FROM	[CP_SupPriceList].[CustomerSupplierMapping]  as c WITH(NOLOCK) 
END