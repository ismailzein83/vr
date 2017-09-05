

CREATE procedure [Mediation_Generic].[sp_MultiLegSessionID_GetByMediationDefinitionID]
@MediationDefinitionID uniqueidentifier
as
begin

DECLARE @MediationDefinitionID_Local uniqueidentifier

SELECT @MediationDefinitionID_Local = @MediationDefinitionID

	SELECT	[MediationDefinitionID],[SessionID],[LegID]
	FROM	[Mediation_Generic].[MultiLegSessionID] with(nolock)
	Where	MediationDefinitionID = @MediationDefinitionID_Local	
end