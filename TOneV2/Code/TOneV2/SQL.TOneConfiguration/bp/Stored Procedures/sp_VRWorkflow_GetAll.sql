CREATE PROCEDURE  [bp].[sp_VRWorkflow_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT	[ID],Name,Title,[Settings],[CreatedTime],[CreatedBy],[LastModifiedTime],[LastModifiedBy]
	FROM	[bp].[VRWorkflow] WITH(NOLOCK)
END