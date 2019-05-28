CREATE TABLE [CRMFixedOper].[ContractService] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [Contract]         BIGINT           NULL,
    [Service]          UNIQUEIDENTIFIER NULL,
    [Status]           UNIQUEIDENTIFIER NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

