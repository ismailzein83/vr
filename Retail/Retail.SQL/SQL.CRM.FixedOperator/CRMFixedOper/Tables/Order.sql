CREATE TABLE [CRMFixedOper].[Order] (
    [ID]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [CustomerID]        BIGINT           NULL,
    [ContractID]        BIGINT           NULL,
    [TypeID]            UNIQUEIDENTIFIER NULL,
    [StatusID]          UNIQUEIDENTIFIER NULL,
    [ProcessInstanceId] BIGINT           NULL,
    [CreatedTime]       DATETIME         NULL,
    [CreatedBy]         INT              NULL,
    [LastModifiedTime]  DATETIME         NULL,
    [LastModifiedBy]    INT              NULL,
    CONSTRAINT [PK__Order__3214EC2707020F21] PRIMARY KEY CLUSTERED ([ID] ASC)
);

