-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_ANumberSaleCode_Close]
	@ANumberSaleCodeId bigint,		
	@EffectiveOn Datetime
AS

BEGIN 

	Update aNUmberSaleCode set aNumberSaleCode.EED = @EffectiveOn
	from [TOneWhS_BE].[ANumberSaleCode] aNumberSaleCode
	where aNumberSaleCode.ID = @ANumberSaleCodeId
	
END