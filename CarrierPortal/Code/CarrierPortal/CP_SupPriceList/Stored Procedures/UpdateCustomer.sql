
CREATE PROCEDURE [CP_SupPriceList].[UpdateCustomer]
	(
	@customeID int,
	@CustomerName varchar(100),
	@Settings nvarchar(max)
	)
AS
BEGIN
BEGIN TRANSACTION
update [CP_SupPriceList].[Customer]
           set [Name]= @CustomerName
           ,[Settings]= @Settings
           where ID= @customeID
    IF @@ERROR <> 0
	BEGIN
    ROLLBACK
    RAISERROR ('Error in updating Customer.', 16, 1)
    RETURN
    END
	COMMIT
END