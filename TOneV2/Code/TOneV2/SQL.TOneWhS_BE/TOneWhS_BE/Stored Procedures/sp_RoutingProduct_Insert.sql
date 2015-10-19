-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_RoutingProduct_Insert
	@Name nvarchar(255),
	@SaleZonePackageID int,
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN

	Insert into TOneWhS_BE.RoutingProduct ([Name], [SaleZonePackageID], [Settings])
	Values(@Name, @SaleZonePackageID, @Settings)
	
	Set @Id = @@IDENTITY
END