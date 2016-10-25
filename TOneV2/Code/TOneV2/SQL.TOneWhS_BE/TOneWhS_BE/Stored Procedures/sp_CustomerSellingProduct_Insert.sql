-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CustomerSellingProduct_Insert]
	@UpdatedCustomerSellingProducts [TOneWhS_BE].[CustomerSellingProduct] READONLY
	
AS
BEGIN
	DECLARE @CustomerID int;
	DECLARE @SellingProductID int;
	DECLARE @BED datetime;
	DECLARE @IdentityValue INT;
	DECLARE @tempTable TABLE (ID INT, CustomerID INT not null,SellingProductID int not null,BED DateTime not null)

	DECLARE CustomerSellingProduct CURSOR FOR SELECT CustomerID,SellingProductID,BED FROM @UpdatedCustomerSellingProducts where ID=0
	
	OPEN CustomerSellingProduct

	FETCH NEXT FROM CustomerSellingProduct INTO @CustomerID,@SellingProductID,@BED


	WHILE @@FETCH_STATUS = 0 
		BEGIN
			Insert into TOneWhS_BE.CustomerSellingProduct([CustomerID] ,[SellingProductID],[BED])
			VALUES (@CustomerID,@SellingProductID,@BED)
			Set @IdentityValue = SCOPE_IDENTITY()
			Insert into @tempTable(ID,[CustomerID] ,[SellingProductID],[BED])
			VALUES (@IdentityValue,@CustomerID,@SellingProductID,@BED)
			FETCH NEXT FROM CustomerSellingProduct INTO @CustomerID,@SellingProductID,@BED
		END

	CLOSE CustomerSellingProduct  
		--Delete From TOneWhS_BE.CustomerSellingProduct
		--Where ID in (select ID from @UpdatedCustomerSellingProducts)
		
select *  from @tempTable tt;
END