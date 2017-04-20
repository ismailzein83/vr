
CREATE procedure [Mediation_Generic].[sp_MultiLegSessionID_DeleteSessionId]
@MediationDefinionID uniqueidentifier,
@SessionID varchar(200)
as
begin

 delete
 FROM	Mediation_Generic.MultiLegSessionID 
 where	MediationDefinitionID = @MediationDefinionID and SessionID = @SessionID

end