-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_SalePriceList_Insert]
	@OwnerType INT,
	@OwnerId INT,
	@CurrencyId INT = NULL,
	@Id INT OUT
AS
BEGIN
	INSERT INTO TOneWhS_BE.SalePriceList (OwnerType, OwnerID, CurrencyID)
	VALUES(@OwnerType, @OwnerId, @CurrencyId)
	
	SET @Id = SCOPE_IDENTITY()
END