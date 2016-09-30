-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_GenericBusinessEntity_GetByDefinition]
		@BusinessEntityDefinitionID uniqueidentifier
AS
BEGIN
	SELECT	gbe.ID,gbe.BusinessEntityDefinitionID,gbe.Details 
	FROM	genericdata.GenericBusinessEntity gbe WITH(NOLOCK) 
	WHERE	gbe.BusinessEntityDefinitionID=@BusinessEntityDefinitionID

END