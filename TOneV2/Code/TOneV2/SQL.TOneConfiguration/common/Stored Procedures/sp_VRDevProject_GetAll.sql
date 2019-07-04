
Create PROCEDURE [common].[sp_VRDevProject_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	ID,Name,CreatedTime,LastModifiedTime
	FROM	[common].VRDevProject  as c WITH(NOLOCK) 
END