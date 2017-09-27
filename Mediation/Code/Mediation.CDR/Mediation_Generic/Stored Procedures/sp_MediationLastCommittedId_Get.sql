

CREATE procedure [Mediation_Generic].[sp_MediationLastCommittedId_Get]
@MediationDefinitionId uniqueidentifier
as
begin

DECLARE @MediationDefinitionId_Local uniqueidentifier
SELECT @MediationDefinitionId_Local = @MediationDefinitionId


    SELECT LastCommittedID FROM [Mediation_Generic].[MediationLastCommittedId] with(nolock)
                   WHERE MediationDefinitionId = @MediationDefinitionId_Local
end