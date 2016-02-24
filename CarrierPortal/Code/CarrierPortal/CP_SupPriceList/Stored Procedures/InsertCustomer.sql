-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [CP_SupPriceList].[InsertCustomer] 
	(
	@CustomerName varchar(100),
	@Settings nvarchar(max),
	@customerID int out
	)
AS
BEGIN
	BEGIN TRANSACTION
INSERT INTO [CP_SupPriceList].[Customer]
           ([Name]
           ,[Settings]
           ,[CreatedTime])
     VALUES
           (@CustomerName,
           @Settings,
           GETDATE())
    IF @@ERROR <> 0
	BEGIN
    ROLLBACK
    RAISERROR ('Error in inserting in Customer.', 16, 1)
    RETURN
    END
	set @customerID = SCOPE_IDENTITY()
	COMMIT
END