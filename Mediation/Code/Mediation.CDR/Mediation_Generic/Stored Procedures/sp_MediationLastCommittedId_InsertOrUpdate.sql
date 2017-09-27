

CREATE procedure [Mediation_Generic].[sp_MediationLastCommittedId_InsertOrUpdate]
@MediationDefinitionId uniqueidentifier,
@LastCommittedId bigint
as
BEGIN


DECLARE @MediationDefinitionId_Local uniqueidentifier, @LastCommittedId_Local bigint
SELECT @MediationDefinitionId_Local = @MediationDefinitionId, @LastCommittedId_Local = @LastCommittedId

	IF NOT EXISTS (SELECT TOP 1 NULL FROM [Mediation_Generic].[MediationLastCommittedId] WITH(NOLOCK) WHERE MediationDefinitionId = @MediationDefinitionId_Local)
	BEGIN
		INSERT INTO [Mediation_Generic].[MediationLastCommittedId] 
		(MediationDefinitionId, LastCommittedID) 
		SELECT @MediationDefinitionId_Local, 0 
		WHERE NOT EXISTS (SELECT TOP 1 NULL FROM [Mediation_Generic].[MediationLastCommittedId] WHERE MediationDefinitionId = @MediationDefinitionId_Local)
	END

	UPDATE [Mediation_Generic].[MediationLastCommittedId]
	SET LastCommittedID = @LastCommittedId_Local
	WHERE MediationDefinitionId = @MediationDefinitionId_Local and LastCommittedID < @LastCommittedId_Local

END