
CREATE procedure [Mediation_Generic].[sp_MultiLegSessionID_InsertSessionLegs]
@MediationDefinitionID uniqueidentifier,
@SessionID varchar(200),
@NonAssociatedLegIds nvarchar(max)
as
begin
	insert into [Mediation_Generic].[MultiLegSessionID]
	select MediationDefinitionId,SessionId,ParsedLegId, getdate() from [Mediation_Generic].[GetLegIdsSessionTable](@MediationDefinitionID, @SessionID, @NonAssociatedLegIds)
end