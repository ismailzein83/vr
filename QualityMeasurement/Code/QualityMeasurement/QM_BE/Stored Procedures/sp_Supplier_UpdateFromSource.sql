CREATE PROCEDURE [QM_BE].[sp_Supplier_UpdateFromSource]
	@ID int,
	@Name nvarchar(255)
AS
BEGIN

	Update QM_BE.Supplier
	Set
	 Name = @Name
	Where ID = @ID

END