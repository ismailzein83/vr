-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_HasRunningInstance]
@definitionID uniqueidentifier,
	@entityIDs varchar(max),
	@Statuses varchar(max)
AS
BEGIN
DECLARE @StatusesTable TABLE ([Status] int)
	INSERT INTO @StatusesTable ([Status])
	SELECT Convert(int, ParsedString) FROM bp.[ParseStringList](@Statuses)
DECLARE @EntityIdsTable TABLE ([EntityId] varchar(10))
	INSERT INTO @EntityIdsTable ([EntityId])
	SELECT Convert(varchar(10), ParsedString) FROM bp.[ParseStringList](@entityIDs)	
  IF Exists ( SELECT top 1 null 	FROM	bp.[BPInstance] bp WITH(NOLOCK)
	JOIN @StatusesTable statuses ON bp.ExecutionStatus = statuses.[Status] WHERE	DefinitionID = @definitionID AND EntityId in (select EntityId from @EntityIdsTable))
	SELECT 1
	ELSE SELECT 0
  	
END