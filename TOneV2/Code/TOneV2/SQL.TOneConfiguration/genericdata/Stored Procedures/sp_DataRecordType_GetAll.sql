CREATE PROCEDURE [genericdata].[sp_DataRecordType_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	[ID],[Name],ParentID,Fields
    FROM	[genericdata].DataRecordType WITH(NOLOCK) 
	ORDER BY [Name]
END