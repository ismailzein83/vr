-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_HasRunningInstance]
@definitionID uniqueidentifier,
	@entityID varchar(255),
	@Statuses varchar(max)
AS
BEGIN
DECLARE @StatusesTable TABLE ([Status] int)
	INSERT INTO @StatusesTable ([Status])
	SELECT Convert(int, ParsedString) FROM bp.[ParseStringList](@Statuses)
	
  IF Exists ( SELECT top 1 null 	FROM	bp.[BPInstance] bp WITH(NOLOCK)
	JOIN @StatusesTable statuses ON bp.ExecutionStatus = statuses.[Status] WHERE	DefinitionID = @definitionID AND EntityId=@entityID)
	SELECT 1
	ELSE SELECT 0
  	
END