create PROCEDURE common.sp_GenericConfiguration_GetAll
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	gc.ID,
			gc.OwnerKey,
			gc.TypeID,
			gc.ConfigDetails
	FROM	[common].GenericConfiguration  as gc WITH(NOLOCK) 
END