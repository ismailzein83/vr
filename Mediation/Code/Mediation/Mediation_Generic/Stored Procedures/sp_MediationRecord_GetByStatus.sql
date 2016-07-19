CREATE procedure [Mediation_Generic].[sp_MediationRecord_GetByStatus]
@MediationDefinitionId int,
@Status tinyint
as
begin
SELECT [EventId]
      ,[SessionId]
      ,[EventTime]
      ,[EventStatus]
	  ,MediationDefinitionId
      ,[EventDetails]
  FROM [Mediation_Generic].[MediationRecord]
  where MediationDefinitionId = @MediationDefinitionId and EventStatus = @Status
end