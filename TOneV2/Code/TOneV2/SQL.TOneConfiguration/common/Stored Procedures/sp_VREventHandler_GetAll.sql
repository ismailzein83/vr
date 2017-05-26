create PROCEDURE [common].sp_VREventHandler_GetAll
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	[ID],[Name],Settings,BED,EED
    from	[common].VREventHandler WITH(NOLOCK) 
	ORDER BY [Name]
END