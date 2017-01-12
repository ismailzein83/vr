﻿-- =============================================
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
	Declare @SupplierId_local INT = @SupplierId
	Declare @When_local DateTime = @When

	SELECT  sz.[ID]
		  ,sz.[Name]
		  ,sz.CountryID
		  ,sz.SupplierID
		  ,sz.BED
		  ,sz.EED
		  ,sz.SourceID 
	  FROM [TOneWhS_BE].SupplierZone sz WITH(NOLOCK)
	  Where (sz.EED is null or sz.EED > @When_local)
	  and sz.SupplierID = @SupplierId_local
	  
END