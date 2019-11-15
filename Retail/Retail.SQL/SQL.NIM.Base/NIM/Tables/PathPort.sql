CREATE TABLE [NIM].[PathPort] (
    [ID]               BIGINT   IDENTITY (1, 1) NOT NULL,
    [Port]             BIGINT   NULL,
    [Path]             BIGINT   NULL,
    [CreatedTime]      DATETIME NULL,
    [CreatedBy]        INT      NULL,
    [LastModifiedTime] DATETIME NULL,
    [LastModifiedBy]   INT      NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

