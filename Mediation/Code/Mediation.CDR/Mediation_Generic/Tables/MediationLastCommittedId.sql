CREATE TABLE [Mediation_Generic].[MediationLastCommittedId] (
    [MediationDefinitionId] UNIQUEIDENTIFIER NOT NULL,
    [LastCommittedID]       BIGINT           NULL,
    CONSTRAINT [PK_MediationLastCommittedId] PRIMARY KEY CLUSTERED ([MediationDefinitionId] ASC)
);

