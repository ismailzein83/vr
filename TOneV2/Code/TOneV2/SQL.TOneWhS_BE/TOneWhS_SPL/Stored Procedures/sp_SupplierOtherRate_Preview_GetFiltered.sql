CREATE PROCEDURE  [TOneWhS_SPL].[sp_SupplierOtherRate_Preview_GetFiltered]
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
	select	 R.ZoneName, R.SystemRate, R.SystemRateBED, R.SystemRateEED, R.ImportedRate, R.ImportedRateBED, R.RateTypeID, R.RateChangeType
	from	TOneWhS_SPL.SupplierOtherRate_Preview as R  with(nolock)
	Where	R.ProcessInstanceID=@ProcessInstanceID
	        AND R.IsExcluded = @IsExcluded
			AND (@ZoneName is null or R.ZoneName = @ZoneName) 
			AND (@OnlyModified = 0 or R.RateChangeType != 0)
	
	SET NOCOUNT OFF
END