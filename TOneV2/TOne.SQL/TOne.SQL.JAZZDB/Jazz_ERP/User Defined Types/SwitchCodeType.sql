CREATE TYPE [Jazz_ERP].[SwitchCodeType] AS TABLE (
    [ID]               UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NULL,
    [Code]             VARCHAR (40)     NULL,
    [CeatedBy]         INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [SwitchId]         INT              NULL);

