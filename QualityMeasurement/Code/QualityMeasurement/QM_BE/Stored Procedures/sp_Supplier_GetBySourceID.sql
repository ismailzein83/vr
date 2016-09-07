


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [QM_BE].[sp_Supplier_GetBySourceID]
	@SourceSupplierID varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [ID]
      ,[Name]
      ,[Settings]
      ,[SourceSupplierID]
      from QM_BE.Supplier WITH(NOLOCK) 
	  Where SourceSupplierID=@SourceSupplierID
END