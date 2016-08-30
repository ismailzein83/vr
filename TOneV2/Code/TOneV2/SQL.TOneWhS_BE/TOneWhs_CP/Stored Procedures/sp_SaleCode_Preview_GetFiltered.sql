
CREATE PROCEDURE  [TOneWhs_CP].[sp_SaleCode_Preview_GetFiltered]
	@ProcessInstanceID_IN INT,
	@ZoneName_IN nvarchar(255),
	@OnlyModified_IN bit
AS
BEGIN
	DECLARE @ProcessInstanceId INT,
	@ZoneName nvarchar(255),
	@OnlyModified bit
	
	SELECT @ProcessInstanceId  = @ProcessInstanceId_IN,
	@ZoneName = @ZoneName_IN,
	@OnlyModified = @OnlyModified_IN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	select C.Code, C.ChangeType, C.RecentZoneName, C.ZoneName, C.BED, C.EED
	from TOneWhs_CP.SaleCode_Preview as C WITH(NOLOCK) 
	Join TOneWhS_CP.SaleZone_Preview as Z  WITH(NOLOCK) on C.ZoneName = Z.ZoneName AND Z.ProcessInstanceID= C.ProcessInstanceID
	Where (@ZoneName is null or C.ZoneName = @ZoneName or c.RecentZoneName= @ZoneName) AND (@OnlyModified = 0 or C.ChangeType != 0) And Z.ProcessInstanceID=@ProcessInstanceID
			
	
	SET NOCOUNT OFF
END