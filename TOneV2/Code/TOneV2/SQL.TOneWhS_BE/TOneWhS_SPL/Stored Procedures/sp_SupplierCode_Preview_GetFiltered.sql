CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierCode_Preview_GetFiltered]
	@ProcessInstanceID INT,
	@ZoneName nvarchar(255),
	@OnlyModified bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	select C.Code, C.ChangeType, C.RecentZoneName, C.ZoneName, C.BED, C.EED,
	Case When C.RecentZoneName is not null and Z.CurrentRate != Z.ImportedRate Then 1 ELSE 0 END as PriceChanged
	from TOneWhS_SPL.SupplierCode_Preview as C
	Join TOneWhS_SPL.SupplierZoneRate_Preview as Z on C.ZoneName = Z.ZoneName AND Z.ProcessInstanceID= C.ProcessInstanceID
	Where (@ZoneName is null or C.ZoneName = @ZoneName or c.RecentZoneName= @ZoneName) AND (@OnlyModified = 0 or C.ChangeType != 0) And Z.ProcessInstanceID=@ProcessInstanceID
			
	
	SET NOCOUNT OFF
END