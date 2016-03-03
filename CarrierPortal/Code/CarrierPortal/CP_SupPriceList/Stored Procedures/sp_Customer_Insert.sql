-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [CP_SupPriceList].[sp_Customer_Insert] 
	(
	@CustomerName nvarchar(255),
	@Settings nvarchar(max),
	@customerID int out
	)
AS
BEGIN
set @customerID=0
IF NOT EXISTS(select 1 from  [CP_SupPriceList].[Customer] where Name = @CustomerName)
BEGIN
	INSERT INTO [CP_SupPriceList].[Customer]
           ([Name]
           ,[Settings]
           ,[CreatedTime])
     VALUES
           (@CustomerName,
           @Settings,
           GETDATE())    
	set @customerID = SCOPE_IDENTITY()
	END
END