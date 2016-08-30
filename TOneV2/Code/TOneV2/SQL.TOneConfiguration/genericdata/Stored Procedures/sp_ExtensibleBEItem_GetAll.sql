CREATE PROCEDURE [genericdata].[sp_ExtensibleBEItem_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	[ID],Details
    FROM	[genericdata].ExtensibleBEItem WITH(NOLOCK) 
END