CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierCode_Preview_GetFiltered]
@ProcessInstanceID_IN INT,
@ZoneName_IN nvarchar(255),
@CountryID_IN INT,
@OnlyModified_IN bit
AS
BEGIN
	DECLARE @ProcessInstanceId INT,
	@ZoneName nvarchar(255),
	@CountryID INT,
	@OnlyModified bit
	
	SELECT @ProcessInstanceId  = @ProcessInstanceId_IN,
	@CountryID = @CountryID_IN,
	@ZoneName = @ZoneName_IN,
	@OnlyModified = @OnlyModified_IN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	
	select	C.Code, C.ChangeType, C.RecentZoneName, C.ZoneName, C.BED, C.EED,Case When C.RecentZoneName is not null and Z.SystemRate != Z.ImportedRate Then 1 ELSE 0 END as PriceChanged
	from	TOneWhS_SPL.SupplierCode_Preview as C  with(nolock)
			Join TOneWhS_SPL.SupplierZoneRate_Preview as Z  with(nolock) on Z.ProcessInstanceID= C.ProcessInstanceID and C.ZoneName = Z.ZoneName
	Where	Z.ProcessInstanceID=@ProcessInstanceID
			AND (@ZoneName is null or C.ZoneName = @ZoneName or c.RecentZoneName= @ZoneName) 
			AND (@OnlyModified = 0 or C.ChangeType != 0)
			AND (@CountryID is null or Z.CountryID = @CountryID)
	SET NOCOUNT OFF
END