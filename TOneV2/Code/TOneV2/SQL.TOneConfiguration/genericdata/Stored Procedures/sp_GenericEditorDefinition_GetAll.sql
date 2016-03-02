create PROCEDURE [genericdata].[sp_GenericEditorDefinition_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [ID]
		   ,[BusinessEntityID]
		   ,Details
      FROM genericdata.GenericEditorDefinition
END