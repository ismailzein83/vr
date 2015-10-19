﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierZone_GetBySupplierId] 
	-- Add the parameters for the stored procedure here
	@SupplierId INT ,
	@When DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT  sz.[ID]
		  ,sz.[Name]
		  ,sz.SupplierID
		  ,sz.BED
		  ,sz.EED
	  FROM [TOneWhS_BE].SupplierZone sz
	  Where sz.SupplierID=@SupplierId
	  and ((sz.BED <= @when ) and (sz.EED is null or sz.EED > @when))
END