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
@CarrierAccountID nvarchar(500),
@resultIdToBeExcluded int,
@priceLisID int out

AS
BEGIN
set @priceLisID = 0
    IF NOT EXISTS(select 1 from  [CP_SupPriceList].[PriceList] where Result = @resultIdToBeExcluded and CustomerID=@CustomerID and CarrierAccountID= @CarrierAccountID)
    begin
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
     set @priceLisID = SCOPE_IDENTITY()
     end
END