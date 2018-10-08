CREATE TYPE [NP_IVSwitch].[PoolBasedCLIGroupType] AS TABLE (
    [ID]               BIGINT         NULL,
    [Name]             NVARCHAR (MAX) NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [CLIPatterns]      NVARCHAR (MAX) NULL);

