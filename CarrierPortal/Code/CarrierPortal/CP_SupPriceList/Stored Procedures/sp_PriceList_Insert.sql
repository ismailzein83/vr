-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [CP_SupPriceList].[sp_PriceList_Insert]
@UserID int,
@FileID int,
@PriceListType int,
@status int,
@EffectiveOn datetime,
@CustomerID int,
@CarrierAccountID nvarchar(500)

AS
BEGIN
	INSERT INTO [CP_SupPriceList].[PriceList]
           ([UserID]
           ,[FileID]
           ,[PriceListType]
           ,[Status]
           ,[CreatedTime]
           ,[EffectiveOnDate]
           ,[CustomerID]
           ,[CarrierAccountID])
     VALUES
           (@UserID,@FileID,@PriceListType,@status,GETDATE(),@EffectiveOn,@CustomerID,@CarrierAccountID)
END