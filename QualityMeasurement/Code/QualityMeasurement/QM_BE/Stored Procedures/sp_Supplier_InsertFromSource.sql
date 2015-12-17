CREATE PROCEDURE [QM_BE].[sp_Supplier_InsertFromSource]
	@ID int,
	@Name nvarchar(255),
	@SourceSupplierID varchar(255)
AS

BEGIN
	Insert into QM_BE.Supplier([ID],[Name],[SourceSupplierID])
	Values(@ID,@Name, @SourceSupplierID)
END