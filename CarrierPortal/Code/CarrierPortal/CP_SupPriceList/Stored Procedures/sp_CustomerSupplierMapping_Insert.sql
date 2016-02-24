
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [CP_SupPriceList].[sp_CustomerSupplierMapping_Insert]
@UserID int,
@CustomerID int,
@MappingSettings nvarchar(max),
@Id int out
AS
BEGIN
	INSERT INTO [CP_SupPriceList].[CustomerSupplierMapping]
           ([UserID]
           ,[CustomerID]
           ,[MappingSettings]
           ,[CreatedTime])
     VALUES (@UserID,@CustomerID,@MappingSettings,GETDATE())
     Set @Id = @@IDENTITY
END