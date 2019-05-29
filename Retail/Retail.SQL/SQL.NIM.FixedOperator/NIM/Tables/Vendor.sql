CREATE TABLE [NIM].[Vendor] (
    [Id]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NULL,
    [CreatedBy]        INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK__Vendor__3214EC07145C0A3F] PRIMARY KEY CLUSTERED ([Id] ASC)
);

