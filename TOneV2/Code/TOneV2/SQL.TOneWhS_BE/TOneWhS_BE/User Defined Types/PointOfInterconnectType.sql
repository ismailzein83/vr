CREATE TYPE [TOneWhS_BE].[PointOfInterconnectType] AS TABLE (
    [ID]               BIGINT         NULL,
    [Name]             NVARCHAR (255) NULL,
    [SwitchId]         INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [TrunkDetails]     NVARCHAR (MAX) NULL);

