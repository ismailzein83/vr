CREATE TYPE [common].[VRDynamicAPIModuleType] AS TABLE (
    [ID]               INT           NULL,
    [Name]             VARCHAR (255) NULL,
    [CreatedTime]      DATETIME      NULL,
    [CreatedBy]        INT           NULL,
    [LastModifiedTime] DATETIME      NULL,
    [LastModifiedBy]   INT           NULL);

