CREATE TYPE [Jazz_ERP].[TaxCodeType] AS TABLE (
    [ID]               UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NULL,
    [SwitchId]         INT              NULL,
    [Code]             VARCHAR (40)     NULL,
    [Direction]        INT              NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL);

