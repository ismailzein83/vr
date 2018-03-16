-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_ANumberSupplierCode_Close]
	@ANumberSupplierCodeId bigint,		
	@EffectiveOn Datetime
AS

BEGIN 

	Update aNUmberSupplierCode set aNumberSupplierCode.EED = @EffectiveOn
	from [TOneWhS_BE].[ANumberSupplierCode] aNumberSupplierCode
	where aNumberSupplierCode.ID = @ANumberSupplierCodeId
	
END