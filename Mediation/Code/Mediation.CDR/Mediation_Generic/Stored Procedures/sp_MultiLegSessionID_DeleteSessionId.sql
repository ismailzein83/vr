
CREATE procedure [Mediation_Generic].[sp_MultiLegSessionID_DeleteSessionId]
@MediationDefinionID uniqueidentifier,
@SessionID varchar(200)
as
begin


DECLARE @MediationDefinitionId_local uniqueidentifier,
@SessionID_Local varchar(200)

SELECT @MediationDefinitionId_local = @MediationDefinionID,
@SessionID_Local = @SessionID

 delete
 FROM	Mediation_Generic.MultiLegSessionID
 where	MediationDefinitionID = @MediationDefinitionId_local and SessionID = @SessionID_Local

end