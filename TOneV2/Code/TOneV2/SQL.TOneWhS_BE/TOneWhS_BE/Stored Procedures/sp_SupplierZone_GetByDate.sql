-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierZone_GetByDate] 
	-- Add the parameters for the stored procedure here
	@SupplierId INT,
	@When DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT  sz.[ID]
		  ,sz.[Name]
		  ,sz.CountryID
		  ,sz.SupplierID
		  ,sz.BED
		  ,sz.EED
		  ,sz.SourceID 
	  FROM [TOneWhS_BE].SupplierZone sz WITH(NOLOCK)
	  Where (sz.EED is null or sz.EED > @when)
	  and sz.SupplierID=@SupplierId
	  
END