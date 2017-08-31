Create procedure [Mediation_Generic].[sp_MediationRecord_GetSessionIdByStatus]
@MediationDefinitionId uniqueidentifier,
@Status tinyint
as
begin
SELECT [SessionId]

  FROM [Mediation_Generic].[MediationRecord] WITH(NOLOCK) 
  where MediationDefinitionId = @MediationDefinitionId and EventStatus = @Status and SessionId is not null
end