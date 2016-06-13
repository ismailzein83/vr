CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierCountry_Preview_CreateTempByFiltered]
	@TempTableName varchar(200),
	@ProcessInstanceId INT,
	@OnlyModified bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			With cteZoneCode AS 
( 
	select Z.ZoneName as ZoneWithCodeChanges

	,SUM(Case When C.ChangeType = 1 Then 1 else 0 END) as NewCodes
	,SUM(Case When C.ChangeType = 3 AND Z.ZoneChangeType != 3 AND C.ZoneName = Z.ZoneName Then 1 else 0 END) as MovedCodes
	,SUM(Case When C.ChangeType = 2 Then 1 else 0 END) as DeleteCodes
	
	 from TOneWhS_SPL.SupplierZoneRate_Preview as Z
	join TOneWhS_SPL.SupplierCode_Preview as C on Z.ZoneName = C.ZoneName OR Z.ZoneName = C.RecentZoneName

	where (@OnlyModified = 0 or C.ChangeType != 0)  and  z.ProcessInstanceID=@ProcessInstanceId and c.ProcessInstanceID=@ProcessInstanceId
	group by Z.ZoneName
)

 Select
  Res.ProcessInstanceID, Res.CountryId
	 ,SUM(Case When Res.ZoneChangeType = 1 Then 1 else 0 END) as NewZones
	 ,SUM(Case When Res.ZoneChangeType = 2 Then 1 else 0 END) as DeletedZones
	 ,SUM(Case When Res.ZoneChangeType = 4  Then 1 else 0 END) as RenamedZones
	 ,SUM(cteZoneCode.NewCodes) as NewCodes
	 ,SUM(cteZoneCode.MovedCodes) as MovedCodes
	 ,SUM(cteZoneCode.DeleteCodes) as DeletedCodes
INTO #RESULT
 from TOneWhS_SPL.SupplierZoneRate_Preview as Res
 Join cteZoneCode on Res.ZoneName = cteZoneCode.ZoneWithCodeChanges
 where res.ProcessInstanceID=@ProcessInstanceId
 
 group by  Res.ProcessInstanceID, Res.CountryId	
 
  	
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END
	SET NOCOUNT OFF
END