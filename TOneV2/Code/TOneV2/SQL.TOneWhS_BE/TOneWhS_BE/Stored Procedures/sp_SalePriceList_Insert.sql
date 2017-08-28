﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePriceList_Insert]
	  @ID int,
      @OwnerType int,
      @OwnerID int,
      @PriceListType int,
      @CurrencyID int,
      @EffectiveOn datetime,
      @ProcessInstanceID bigint,
      @FileID bigint,
	  @UserID int
AS
BEGIN

	BEGIN
		 INSERT INTO [TOneWhS_BE].[SalePriceList]([ID], [OwnerType], [OwnerID], [CurrencyID], [EffectiveOn], [PriceListType], [ProcessInstanceID], [FileID],[UserID])
     VALUES (@ID, @OwnerType, @OwnerID, @CurrencyID, @EffectiveOn, @PriceListType, @ProcessInstanceID, @FileID, @UserID)

	END
END