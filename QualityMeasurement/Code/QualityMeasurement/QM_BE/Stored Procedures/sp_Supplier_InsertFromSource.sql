CREATE PROCEDURE [QM_BE].[sp_Supplier_InsertFromSource]
	@ID int,
	@Name nvarchar(255),
	@Settings nvarchar(MAX),
	@SourceSupplierID varchar(255)
AS

BEGIN
	Insert into QM_BE.Supplier([ID],[Name], [Settings], [SourceSupplierID])
	Values(@ID, @Name, @Settings,  @SourceSupplierID)
END