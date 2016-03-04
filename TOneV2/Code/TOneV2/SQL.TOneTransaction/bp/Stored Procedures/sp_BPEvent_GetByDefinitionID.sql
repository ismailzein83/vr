-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPEvent_GetByDefinitionID]
	@DefinitionID int
AS
BEGIN
	SELECT e.[ID]
      ,e.[ProcessInstanceID]
      ,i.DefinitionID
      ,e.[Bookmark]
      ,e.[Payload]
  FROM [bp].[BPEvent] e  WITH(NOLOCK)
  JOIN bp.BPInstance i  WITH(NOLOCK) on e.ProcessInstanceID = i.ID
  WHERE i.DefinitionID = @DefinitionID
END