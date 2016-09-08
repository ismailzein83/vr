
CREATE procedure [Mediation_Generic].[sp_MediationRecord_GetByIds]
@Ids dbo.StringIDType READONLY,
@MediationDefinitionId int
as
begin
SELECT [EventId]
      ,sr.[SessionId]
      ,[EventTime]
      ,[EventStatus]
	  ,MediationDefinitionId
      ,[EventDetails]
  FROM [Mediation_Generic].[MediationRecord] sr WITH(NOLOCK) 
   JOIN @Ids i ON sr.SessionId = i.SessionId  
   where sr.MediationDefinitionId = @MediationDefinitionId
   order by sr.SessionId

end