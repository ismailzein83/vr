CREATE PROCEDURE [QM_BE].[sp_Supplier_Insert]
	@ID int,
	@Name nvarchar(255),
	@Settings nvarchar(max)
AS
BEGIN
IF NOT EXISTS(select 1 from  QM_BE.Supplier where Name = @Name and (IsDeleted is null or IsDeleted=0))
BEGIN
	Insert into QM_BE.Supplier( [ID],[Name],[Settings])
	Values(@ID,@Name,@Settings)
	
	END
END