CREATE TABLE [NIM].[SiteType] (
    [Id]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NULL,
    [CreatedBy]        INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK__SiteType__3214EC071BFD2C07] PRIMARY KEY CLUSTERED ([Id] ASC)
);

