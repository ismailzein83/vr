CREATE procedure [Mediation_Generic].[sp_MediationRecord_GetSessionIdByStatus]
@MediationDefinitionId uniqueidentifier,
@Status tinyint,
@LastCommittedId bigint
as
begin

DECLARE @MediationDefinitionId_local uniqueidentifier,
@Status_Local tinyint,
@LastCommittedId_Local bigint

SELECT @MediationDefinitionId_local = @MediationDefinitionId,
@Status_Local = @Status,
@LastCommittedId_Local  = @LastCommittedId


SELECT [SessionId]

  FROM [Mediation_Generic].[MediationRecord] WITH(NOLOCK) 
  where MediationDefinitionId = @MediationDefinitionId_local and EventStatus = @Status_local and EventId <= @LastCommittedId_Local and SessionId is not null
  ORDER BY EventId
end