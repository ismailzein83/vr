CREATE TYPE [Jazz_ERP].[CustomerTypeType] AS TABLE (
    [ID]              UNIQUEIDENTIFIER NULL,
    [Name]            NVARCHAR (255)   NULL,
    [Code]            VARCHAR (40)     NULL,
    [CreatedBy]       INT              NULL,
    [CreatedTime]     DATETIME         NULL,
    [LastCreatedBy]   INT              NULL,
    [LastCreatedTime] DATETIME         NULL);

