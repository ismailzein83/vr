

CREATE PROCEDURE  [TOneWhs_CP].[sp_SaleZoneRoutingProduct_Preview_GetFiltered]
	@ProcessInstanceID_IN INT,
	@ZoneName_IN nvarchar(255),
	@OnlyModified_IN bit
AS
BEGIN
	DECLARE @ProcessInstanceId INT,
	@ZoneName nvarchar(255),
	@OnlyModified bit=0

	SELECT @ProcessInstanceId  = @ProcessInstanceId_IN,
	@ZoneName = @ZoneName_IN,
	@OnlyModified=@OnlyModified_IN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	select ZoneName , OwnerType , OwnerID , RoutingProductID , BED , EED,ChangeType
	from TOneWhs_CP.SaleZoneRoutingProduct_Preview WITH(NOLOCK) 
	Where (@ZoneName is null or ZoneName = @ZoneName) and ProcessInstanceID = @ProcessInstanceId and (@OnlyModified = 0 or ChangeType != 0)
			
	
	SET NOCOUNT OFF
END