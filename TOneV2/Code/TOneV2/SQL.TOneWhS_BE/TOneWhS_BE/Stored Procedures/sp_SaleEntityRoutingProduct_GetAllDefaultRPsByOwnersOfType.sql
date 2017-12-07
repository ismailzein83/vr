-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_SaleEntityRoutingProduct_GetAllDefaultRPsByOwnersOfType
	@OwnerType int,
	@OwnerIds nvarchar(max)
AS
BEGIN
	declare @OwnerIdsTable as table (OwnerId int)
	if (@OwnerIds is not null) begin insert into @OwnerIdsTable select convert(int, ParsedString) from TOneWhS_BE.ParseStringList(@OwnerIds) end

	select [ID], [OwnerType], [OwnerID], [RoutingProductID], [BED], [EED]
	from TOneWhS_BE.SaleEntityRoutingProduct with(nolock)
	where ZoneID is null and OwnerType = @OwnerType and OwnerID in (select OwnerId from @OwnerIdsTable)
END