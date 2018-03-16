CREATE PROCEDURE [TOneWhS_BE].[sp_CarrierProfile_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	cp.ID,
			cp.Name,
			cp.Settings,
			cp.SourceID,
			cp.IsDeleted,
			cp.ExtendedSettings,
			cp.CreatedTime,
			cp.CreatedBy,
			cp.LastModifiedBy,
			cp.LastModifiedTime
	FROM	[TOneWhS_BE].CarrierProfile  as cp WITH(NOLOCK) 
END