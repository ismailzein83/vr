CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierCountry_Preview_GetFiltered]
	@ProcessInstanceId_IN INT,
	@OnlyModified_IN bit
AS
BEGIN
	DECLARE @ProcessInstanceId INT,
	@OnlyModified bit
	
	SELECT @ProcessInstanceId  = @ProcessInstanceId_IN,
	@OnlyModified = @OnlyModified_IN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	

With cteZoneCode AS 
( 
	select Z.ZoneName as ZoneWithCodeChanges

	,SUM(Case When C.ChangeType = 1 Then 1 else 0 END) as NewCodes
	,SUM(Case When C.ChangeType = 3 AND Z.ZoneChangeType != 2 AND C.ZoneName = Z.ZoneName Then 1 else 0 END) as MovedCodes
	,SUM(Case When C.ChangeType = 2 Then 1 else 0 END) as DeleteCodes
	
	 from TOneWhS_SPL.SupplierZoneRate_Preview as Z WITH(NOLOCK) 
	join TOneWhS_SPL.SupplierCode_Preview as C WITH(NOLOCK)  on Z.ZoneName = C.ZoneName OR Z.ZoneName = C.RecentZoneName
	left outer join TOneWhS_SPL.SupplierOtherRate_Preview Orp WITH(NOLOCK)   on Orp.ZoneName = Z.ZoneName

	where (@OnlyModified = 0 or C.ChangeType != 0 or z.RateChangeType != 0 or Orp.RateChangeType != 0)  and  z.ProcessInstanceID=@ProcessInstanceId and c.ProcessInstanceID=@ProcessInstanceId
	group by Z.ZoneName
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
 Join cteZoneCode  WITH(NOLOCK) on Res.ZoneName = cteZoneCode.ZoneWithCodeChanges
 where res.ProcessInstanceID=@ProcessInstanceId
 
 group by Res.CountryId	
 
	SET NOCOUNT OFF
END