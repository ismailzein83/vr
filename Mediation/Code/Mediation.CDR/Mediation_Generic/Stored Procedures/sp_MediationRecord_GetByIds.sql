

CREATE procedure [Mediation_Generic].[sp_MediationRecord_GetByIds]
@Ids dbo.StringIDType READONLY,
@MediationDefinitionId uniqueidentifier
as
begin

DECLARE @MediationDefinitionId_Local uniqueidentifier,

@Ids_Local dbo.StringIDType 

SELECT @MediationDefinitionId_Local = @MediationDefinitionId

INSERT INTO @Ids_Local
SELECT * FROM @Ids

SELECT [EventId]
      ,sr.[SessionId]
      ,[EventTime]
      ,[EventStatus]
	  ,MediationDefinitionId
      ,[EventDetails]
  FROM [Mediation_Generic].[MediationRecord] sr WITH(NOLOCK) 
  JOIN @Ids_Local i ON sr.MediationDefinitionId = @MediationDefinitionId_Local AND sr.SessionId  = i.SessionId
   
end