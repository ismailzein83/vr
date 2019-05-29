CREATE TABLE [NIM_PSTN].[Cable] (
    [Id]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NULL,
    [MDF]              BIGINT         NULL,
    [CreatedBy]        INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK__Cable__3214EC0760A75C0F] PRIMARY KEY CLUSTERED ([Id] ASC)
);

