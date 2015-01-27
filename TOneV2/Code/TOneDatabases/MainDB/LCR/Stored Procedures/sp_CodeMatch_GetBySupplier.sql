﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [LCR].[sp_CodeMatch_GetBySupplier] 
	@SupplierID varchar(5),
	@IsFuture bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [Code]
      ,[SupplierCode]
      ,[SupplierCodeID]
      ,[SupplierZoneID]
	FROM [LCR].[CodeMatch] WITH (NOLOCK)
	WHERE [SupplierID] = @SupplierID AND [IsFuture] = @IsFuture
END