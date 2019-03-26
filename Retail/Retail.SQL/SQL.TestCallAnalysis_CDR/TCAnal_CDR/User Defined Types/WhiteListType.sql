CREATE TYPE [TCAnal_CDR].[WhiteListType] AS TABLE (
    [ID]               BIGINT       NULL,
    [OperatorId]       BIGINT       NULL,
    [Code]             VARCHAR (40) NULL,
    [CreatedTime]      DATETIME     NULL,
    [LastModifiedTime] DATETIME     NULL,
    [CreatedBy]        INT          NULL,
    [LastModifiedBy]   INT          NULL);

