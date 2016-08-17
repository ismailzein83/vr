
CREATE PROCEDURE  [TOneWhs_CP].[sp_SaleRate_Preview_GetFiltered]
	@ProcessInstanceID_IN INT,
	@ZoneName_IN nvarchar(255)
AS
BEGIN
	DECLARE @ProcessInstanceId INT,
	@ZoneName nvarchar(255)
	
	SELECT @ProcessInstanceId  = @ProcessInstanceId_IN,
	@ZoneName = @ZoneName_IN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	select R.ZoneName , R.OwnerType , R.OwnerID , R.Rate , R.BED , R.EED
	from TOneWhs_CP.SaleRate_Preview as R
	Where (@ZoneName is null or R.ZoneName = @ZoneName)
			
	
	SET NOCOUNT OFF
END