CREATE TYPE [Jazz_ERP].[AccountCodeType] AS TABLE (
    [ID]                UNIQUEIDENTIFIER NULL,
    [Name]              NVARCHAR (255)   NULL,
    [SwitchId]          INT              NULL,
    [TransactionTypeId] UNIQUEIDENTIFIER NULL,
    [Code]              VARCHAR (40)     NULL,
    [CreatedBy]         INT              NULL,
    [CreatedTime]       DATETIME         NULL,
    [LastModifiedBy]    INT              NULL,
    [LastModifiedTime]  DATETIME         NULL,
    [Carriers]          NVARCHAR (MAX)   NULL);

