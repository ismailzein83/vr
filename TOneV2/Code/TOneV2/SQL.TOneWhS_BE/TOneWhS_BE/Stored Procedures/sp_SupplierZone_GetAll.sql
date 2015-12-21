﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierZone_GetAll] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT sz.[ID]
		  ,sz.[Name]
		  ,sz.SupplierID
		  ,sz.BED
		  ,sz.EED
		  ,sz.CountryID
	  FROM [TOneWhS_BE].SupplierZone sz
	
END