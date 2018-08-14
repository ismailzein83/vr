CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierZonesServices_Preview_GetFiltered]
@ProcessInstanceID_IN INT,
@ZoneName_IN nvarchar(255),
@OnlyModified_IN bit,
@IsExcluded_IN bit
AS
BEGIN
	DECLARE @ProcessInstanceId INT,
	@ZoneName nvarchar(255),
	@OnlyModified bit,
	@IsExcluded bit
	
	SELECT @ProcessInstanceId  = @ProcessInstanceId_IN,
	@ZoneName = @ZoneName_IN,
	@OnlyModified = @OnlyModified_IN,
	@IsExcluded = @IsExcluded_IN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	
	select	 zs.ZoneName, zs.SystemZoneServiceIds, zs.SystemZoneServiceBED, zs.SystemZoneServiceEED, zs.ImportedZoneServiceIds,
	 zs.ImportedZoneServiceBED, zs.ZoneServiceChangeType
	from	TOneWhS_SPL.SupplierZoneRate_Preview as zs  with(nolock)
	Where	zs.ProcessInstanceID=@ProcessInstanceID
	        AND zs.IsExcluded = @IsExcluded
			AND (@ZoneName is null or zs.ZoneName = @ZoneName) 
			AND (@OnlyModified = 0 or zs.ZoneServiceChangeType != 0)
	
	SET NOCOUNT OFF
END