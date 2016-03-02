
CREATE PROCEDURE [CP_SupPriceList].[sp_Customer_Update]
	(
	@customeID int,
	@CustomerName varchar(100),
	@Settings nvarchar(max)
	)
AS
BEGIN
--IF NOT EXISTS(select 1 from  [CP_SupPriceList].[Customer] where Name = @CustomerName)
begin
update [CP_SupPriceList].[Customer]
           set [Name]= @CustomerName
           ,[Settings]= @Settings
           where ID= @customeID
   
END
end