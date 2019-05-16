CREATE TABLE [NIM].[LAC] (
    [Id]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Code]             NVARCHAR (255) NULL,
    [CreatedBy]        INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [timestamp]        ROWVERSION     NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

