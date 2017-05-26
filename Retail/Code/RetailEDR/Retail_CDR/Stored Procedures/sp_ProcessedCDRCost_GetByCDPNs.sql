CREATE PROCEDURE [Retail_CDR].[sp_ProcessedCDRCost_GetByCDPNs]
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

	SELECT  [ID],[AttemptDateTime],[CGPN],[CDPN],[DurationInSeconds],[Rate],[Amount]
	FROM	[Retail_CDR].[ProcessedCDRCost] WITH(NOLOCK) 
	WHERE	[AttemptDateTime] >= @From and [AttemptDateTime] < @To and [CDPN] in (select CDPN from @CDPNsTable)
END