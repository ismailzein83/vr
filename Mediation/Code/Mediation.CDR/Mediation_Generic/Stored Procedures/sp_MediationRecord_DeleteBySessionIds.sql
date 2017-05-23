CREATE procedure [Mediation_Generic].[sp_MediationRecord_DeleteBySessionIds]
@Ids dbo.StringIDType READONLY,
@MediationDefinitionId uniqueidentifier
as
begin
delete sr
  FROM [Mediation_Generic].[MediationRecord] sr
   JOIN @Ids i ON sr.SessionId = i.SessionId  
   where sr.MediationDefinitionId = @MediationDefinitionId

end