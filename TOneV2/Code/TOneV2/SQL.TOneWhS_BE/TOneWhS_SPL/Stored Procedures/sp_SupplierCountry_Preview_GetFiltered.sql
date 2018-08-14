CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierCountry_Preview_GetFiltered]
	@ProcessInstanceId_IN INT,
	@OnlyModified_IN bit,
	@IsExcluded_IN bit
AS
BEGIN
	DECLARE @ProcessInstanceId INT,
	@OnlyModified bit,
	@IsExcluded bit
	
	SELECT @ProcessInstanceId  = @ProcessInstanceId_IN,
	@OnlyModified = @OnlyModified_IN,@IsExcluded = @IsExcluded_IN
	
	SET NOCOUNT ON;

With cteZoneCode AS 
( 
	select Z.ZoneName as ZoneWithCodeChanges

	,SUM(Case When C.ChangeType = 1 Then 1 else 0 END) as NewCodes
	,SUM(Case When C.ChangeType = 3 AND Z.ZoneChangeType != 2 AND C.ZoneName = Z.ZoneName Then 1 else 0 END) as MovedCodes
	,SUM(Case When C.ChangeType = 2 Then 1 else 0 END) as DeleteCodes
	
	 from TOneWhS_SPL.SupplierZoneRate_Preview as Z WITH(NOLOCK) 
	join TOneWhS_SPL.SupplierCode_Preview as C WITH(NOLOCK)  on Z.ZoneName = C.ZoneName OR Z.ZoneName = C.RecentZoneName

	where (@OnlyModified = 0 or C.ChangeType != 0 or z.RateChangeType != 0 or z.ZoneServiceChangeType != 0)  and  z.ProcessInstanceID=@ProcessInstanceId and c.ProcessInstanceID=@ProcessInstanceId and z.IsExcluded=@IsExcluded and c.IsExcluded=@IsExcluded
	group by Z.ZoneName
),


cteZoneOtherRate AS 
( 
SELECT sor.ZoneName from TOneWhS_SPL.SupplierOtherRate_Preview sor
Where sor.ProcessInstanceID =@ProcessInstanceId  and (@OnlyModified = 0 or sor.RateChangeType != 0)
group by sor.ZoneName
)


 Select
	Res.CountryId
	 ,SUM(Case When Res.ZoneChangeType = 1 Then 1 else 0 END) as NewZones
	 ,SUM(Case When Res.ZoneChangeType = 2 Then 1 else 0 END) as DeletedZones
	 ,SUM(Case When Res.ZoneChangeType = 4  Then 1 else 0 END) as RenamedZones
	 ,SUM(cteZoneCode.NewCodes) as NewCodes
	 ,SUM(cteZoneCode.MovedCodes) as MovedCodes
	 ,SUM(cteZoneCode.DeleteCodes) as DeletedCodes
 from TOneWhS_SPL.SupplierZoneRate_Preview as Res WITH(NOLOCK) 
 left Join cteZoneCode  WITH(NOLOCK) on Res.ZoneName = cteZoneCode.ZoneWithCodeChanges
 left Join cteZoneOtherRate  WITH(NOLOCK) on Res.ZoneName = cteZoneOtherRate.ZoneName
 where res.ProcessInstanceID=@ProcessInstanceId and (cteZoneOtherRate.ZoneName IS NOT NULL or cteZoneCode.ZoneWithCodeChanges IS NOT NULL) and res.IsExcluded = @IsExcluded
 
 group by Res.CountryId	
 
	SET NOCOUNT OFF
END