-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CustomerPricingProduct_Insert]
	@UpdatedCustomerPricingProducts [TOneWhS_BE].[CustomerPricingProduct] READONLY
	
AS
BEGIN
	DECLARE @CustomerID int;
	DECLARE @PricingProductID int;
	DECLARE @AllDestinations bit;
	DECLARE @BED datetime;
	DECLARE @EED datetime;
	DECLARE @IdentityValue INT;
	DECLARE @tempTable TABLE (ID INT, CustomerID INT not null,PricingProductID int not null,AllDestinations bit not null,BED DateTime not null,EED DATETIME null)

	DECLARE CustomerPricingProduct CURSOR FOR SELECT CustomerID,PricingProductID,AllDestinations,BED,EED FROM @UpdatedCustomerPricingProducts where ID=0
	
	OPEN CustomerPricingProduct

	FETCH NEXT FROM CustomerPricingProduct INTO @CustomerID,@PricingProductID,@AllDestinations,@BED,@EED


	WHILE @@FETCH_STATUS = 0 
		BEGIN
			Insert into TOneWhS_BE.CustomerPricingProduct([CustomerID] ,[PricingProductID],[AllDestinations],[BED] ,[EED])
			VALUES (@CustomerID,@PricingProductID,@AllDestinations,@BED,@EED)
			Set @IdentityValue = @@IDENTITY
			Insert into @tempTable(ID,[CustomerID] ,[PricingProductID],[AllDestinations],[BED] ,[EED])
			VALUES (@IdentityValue,@CustomerID,@PricingProductID,@AllDestinations,@BED,@EED)
			FETCH NEXT FROM CustomerPricingProduct INTO @CustomerID,@PricingProductID,@AllDestinations,@BED,@EED
		END

	CLOSE CustomerPricingProduct  

		UPDATE TOneWhS_BE.CustomerPricingProduct
		SET 
		TOneWhS_BE.CustomerPricingProduct.EED=ucpp.EED
		FROM TOneWhS_BE.CustomerPricingProduct  inner join @UpdatedCustomerPricingProducts as ucpp ON  TOneWhS_BE.CustomerPricingProduct.ID = ucpp.ID
		
	select *,ca.Name as CustomerName ,pp.Name as PricingProductName from @tempTable tt 
	LEFT JOIN TOneWhS_BE.CarrierAccount ca ON tt.CustomerID=ca.ID 
	LEFT Join TOneWhS_BE.CarrierProfile cp ON ca.CarrierProfileID=cp.ID
	LEFT Join TOneWhS_BE.PricingProduct pp ON tt.PricingProductID=pp.ID     ;
END