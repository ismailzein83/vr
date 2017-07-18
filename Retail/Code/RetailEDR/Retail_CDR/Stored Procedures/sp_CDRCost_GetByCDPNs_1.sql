CREATE PROCEDURE [Retail_CDR].[sp_CDRCost_GetByCDPNs]
	@From DateTime,
	@To DateTime,
	@CDPNs nvarchar(max)
AS
BEGIN
	declare @CDPNsTable as table (CDPN varchar(100))
	if (@CDPNs is not null) 
	begin 
		insert into @CDPNsTable 
		select convert(varchar(100), ParsedString)
		from [Retail_CDR].[ParseStringList](@CDPNs) 
	end

	SELECT  [ID],[SourceID],[AttemptDateTime],[CGPN],[CDPN],[SupplierName],[DurationInSeconds],[CurrencyId],[Rate],[Amount]
	FROM	[Retail_CDR].[CDRCost] WITH(NOLOCK) 
	WHERE	[AttemptDateTime] >= @From 
			AND [AttemptDateTime] <= @To 
			AND [CDPN] in (select CDPN from @CDPNsTable)
			AND ISNULL(IsDeleted, 0) <> 1
END