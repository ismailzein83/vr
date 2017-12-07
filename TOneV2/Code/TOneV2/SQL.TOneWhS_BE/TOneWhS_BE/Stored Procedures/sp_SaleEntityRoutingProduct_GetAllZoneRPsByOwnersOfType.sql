-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetAllZoneRPsByOwnersOfType]
	@OwnerType int,
	@OwnerIds nvarchar(max),
	@ZoneIds nvarchar(max)
AS
BEGIN
	declare @OwnerIdsTable as table (OwnerId int)
	if (@OwnerIds is not null) begin insert into @OwnerIdsTable select convert(int, ParsedString) from TOneWhS_BE.ParseStringList(@OwnerIds) end

	declare @ZoneIdsTable as table (ZoneId bigint)
	if (@ZoneIds is not null) begin insert into @ZoneIdsTable select convert(bigint, ParsedString) from TOneWhS_BE.ParseStringList(@ZoneIds) end

	select [ID], [OwnerType], [OwnerID], [ZoneID], [RoutingProductID], [BED], [EED]
	from TOneWhS_BE.SaleEntityRoutingProduct with(nolock)
	where OwnerType = @OwnerType and OwnerID in (select OwnerId from @OwnerIdsTable) and ZoneID is not null and ZoneID in (select ZoneId from @ZoneIdsTable)
END