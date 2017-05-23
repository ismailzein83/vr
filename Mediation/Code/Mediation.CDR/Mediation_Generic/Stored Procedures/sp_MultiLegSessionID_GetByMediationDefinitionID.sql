

CREATE procedure [Mediation_Generic].[sp_MultiLegSessionID_GetByMediationDefinitionID]
@MediationDefinitionID uniqueidentifier
as
begin
	SELECT	[MediationDefinitionID],[SessionID],[LegID]
	FROM	[Mediation_Generic].[MultiLegSessionID]
	Where	MediationDefinitionID = @MediationDefinitionID	
end