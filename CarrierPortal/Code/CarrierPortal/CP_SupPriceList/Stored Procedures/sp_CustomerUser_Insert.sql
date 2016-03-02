-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [CP_SupPriceList].[sp_CustomerUser_Insert]
(
	@customerId int,
	@userId int
)
AS
BEGIN
IF NOT EXISTS(select 1 from  [CP_SupPriceList].[CustomerUser] where UserID = @userId)
BEGIN
	INSERT INTO [CP_SupPriceList].[CustomerUser]
           ([UserID]
           ,[CustomerID]
           ,[CreatedTime])
     VALUES
           (@userId,
           @customerId,
           GETDATE())
END
END