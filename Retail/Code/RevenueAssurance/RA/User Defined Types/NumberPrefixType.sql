CREATE TYPE [RA].[NumberPrefixType] AS TABLE (
    [ID]               BIGINT       NULL,
    [Code]             VARCHAR (20) NULL,
    [BED]              DATETIME     NULL,
    [EED]              DATETIME     NULL,
    [MobileNetworkId]  INT          NULL,
    [CreatedBy]        INT          NULL,
    [CreatedTime]      DATETIME     NULL,
    [LastModifiedBy]   INT          NULL,
    [LastModifiedTime] DATETIME     NULL);

