
CREATE PROCEDURE [genericdata].[sp_SummaryTransformationDefinition_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	def.[ID],def.[Name],def.Details,def.CreatedTime
    FROM	genericdata.SummaryTransformationDefinition def WITH(NOLOCK) 
END