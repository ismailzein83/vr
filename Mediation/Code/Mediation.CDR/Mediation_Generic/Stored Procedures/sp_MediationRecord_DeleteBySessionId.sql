



CREATE procedure [Mediation_Generic].[sp_MediationRecord_DeleteBySessionId]
@MediationDefinitionId uniqueidentifier,
@SessionId nvarchar(255)
as
begin
delete sr
  FROM [Mediation_Generic].[MediationRecord] sr
   where sr.MediationDefinitionId = @MediationDefinitionId and SessionId = @SessionId

end