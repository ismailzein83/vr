CREATE TABLE [Jazz_ERP].[TransactionType] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_TransactionType_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    [CarrierType]      INT              NULL,
    [IsCredit]         BIT              NULL,
    [TransactionScope] INT              NULL,
    CONSTRAINT [PK_TransactionType] PRIMARY KEY CLUSTERED ([ID] ASC)
);

