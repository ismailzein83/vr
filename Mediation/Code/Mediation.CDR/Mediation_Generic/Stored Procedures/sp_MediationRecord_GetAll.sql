CREATE procedure [Mediation_Generic].[sp_MediationRecord_GetAll]
as
begin
SELECT [EventId]
      ,[SessionId]
      ,[EventTime]
      ,[EventStatus]
	  ,MediationDefinitionId
      ,[EventDetails]
  FROM Mediation_Generic.[MediationRecord] WITH(NOLOCK) 
end