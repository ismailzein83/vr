-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPEvent_GetByInstanceIDs]
	@DefinitionID int,
	@InstanceIDs varchar(max)
AS
BEGIN
	DECLARE @InstanceIDsTable TABLE (ID BIGINT)
	IF @InstanceIDs IS NOT NULL
	BEGIN
		INSERT INTO @InstanceIDsTable (ID)
		SELECT Convert(BIGINT, ParsedString) FROM bp.[ParseStringList](@InstanceIDs)
	END
	
	SELECT e.[ID]
      ,e.[ProcessInstanceID]
      ,i.DefinitionID
      ,e.[Bookmark]
      ,e.[Payload]
  FROM [bp].[BPEvent] e  WITH(NOLOCK)
  JOIN @InstanceIDsTable runningInstances ON e.ProcessInstanceID = runningInstances.ID
  JOIN bp.BPInstance i  WITH(NOLOCK) on e.ProcessInstanceID = i.ID
  WHERE i.DefinitionID = @DefinitionID
END