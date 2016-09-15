CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierZoneRate_Preview_GetFiltered]
	@ProcessInstanceID_IN INT,
	@CountryID_IN INT,
	@OnlyModified_IN bit
AS
BEGIN
	DECLARE @ProcessInstanceId INT,
	@CountryID INT,
	@OnlyModified bit
	
	SELECT @ProcessInstanceId  = @ProcessInstanceId_IN,
	@CountryID = @CountryID_IN,
	@OnlyModified = @OnlyModified_IN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


With cteZoneOtherRate AS 
( 
SELECT sor.ZoneName from TOneWhS_SPL.SupplierOtherRate_Preview sor
Where sor.ProcessInstanceID =@ProcessInstanceId  and (@OnlyModified = 0 or sor.RateChangeType != 0)
group by sor.ZoneName
)

	select	Z.ZoneName,Z.RecentZoneName, Z.ZoneChangeType, Z.ZoneBED, Z.ZoneEED, Z.SystemRate, Z.SystemRateBED, Z.SystemRateEED, 
			Z.ImportedRate, Z.ImportedRateBED, Z.RateChangeType,
			SUM(Case When C.ChangeType = 1 Then 1 else 0 END) as NewCodes,
			SUM(Case When C.ChangeType = 2 Then 1 else 0 END) as DeletedCodes,
			SUM(Case When C.ChangeType = 3 AND Z.ZoneChangeType != 2 AND C.ZoneName = Z.ZoneName Then 1 else 0 END) as CodesMovedTo,
			SUM(Case When C.ChangeType = 3 AND Z.ZoneChangeType != 2 AND C.RecentZoneName = Z.ZoneName Then 1 else 0 END) as CodesMovedFrom
	
	from	 cteZoneOtherRate  WITH(NOLOCK) full outer join  TOneWhS_SPL.SupplierZoneRate_Preview as Z  with(nolock) on cteZoneOtherRate.ZoneName =Z.ZoneName 
			join TOneWhS_SPL.SupplierCode_Preview as C  with(nolock) on Z.ZoneName = C.ZoneName OR Z.ZoneName = C.RecentZoneName
			
			 	
	Where	(Z.ProcessInstanceID=@ProcessInstanceID 
			AND c.ProcessInstanceID=@ProcessInstanceID
			AND (@CountryID is null or Z.CountryID = @CountryID) )
			AND (@OnlyModified = 0 or C.ChangeType != 0 or z.RateChangeType != 0 or z.ZoneServiceChangeType != 0 or (cteZoneOtherRate.ZoneName Is Not NULL  and (CountryID is null or Z.CountryID = @CountryID)))
			
					

	Group by Z.ZoneName,Z.RecentZoneName, Z.ZoneChangeType, Z.ZoneBED, Z.ZoneEED, Z.SystemRate, Z.SystemRateBED, Z.SystemRateEED,Z.ImportedRate, Z.ImportedRateBED, Z.RateChangeType
		
	SET NOCOUNT OFF
END