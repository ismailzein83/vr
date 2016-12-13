﻿
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [QM_BE].[sp_Supplier_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [ID]
      ,[Name]
      ,[SourceSupplierID]
      ,[Settings]
      from QM_BE.Supplier WITH(NOLOCK) WHERE IsDeleted = 0 OR IsDeleted IS NULL 
END