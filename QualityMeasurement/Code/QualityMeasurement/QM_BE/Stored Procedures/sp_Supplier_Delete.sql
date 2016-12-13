CREATE PROCEDURE [QM_BE].[sp_Supplier_Delete]
	@ID int
AS
BEGIN
BEGIN
	Update QM_BE.Supplier
	Set
	 IsDeleted = 1
	Where ID = @ID
END
END