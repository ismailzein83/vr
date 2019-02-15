CREATE TYPE [Jazz_ERP].[TransactionTypeType] AS TABLE (
    [ID]               UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NULL,
    [IsCredit]         BIT              NULL,
    [TransactionScope] INT              NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [CarrierType]      INT              NULL);

