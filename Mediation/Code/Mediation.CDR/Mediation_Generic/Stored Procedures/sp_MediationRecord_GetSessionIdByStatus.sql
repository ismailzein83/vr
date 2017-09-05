CREATE procedure [Mediation_Generic].[sp_MediationRecord_GetSessionIdByStatus]
@MediationDefinitionId uniqueidentifier,
@Status tinyint
as
begin

DECLARE @MediationDefinitionId_local uniqueidentifier,
@Status_Local tinyint

SELECT @MediationDefinitionId_local = @MediationDefinitionId,
@Status_Local = @Status

SELECT [SessionId]

  FROM [Mediation_Generic].[MediationRecord] WITH(NOLOCK) 
  where MediationDefinitionId = @MediationDefinitionId_local and EventStatus = @Status_local and SessionId is not null
  ORDER BY EventId
end