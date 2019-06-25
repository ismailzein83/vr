CREATE TABLE [NIM_PSTN].[IMS] (
    [Id]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NULL,
    [Site]             BIGINT         NULL,
    [Vendor]           BIGINT         NULL,
    [Region]           INT            NULL,
    [City]             INT            NULL,
    [Town]             INT            NULL,
    [CreatedBy]        INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [timestamp]        ROWVERSION     NULL,
    [Number]           NVARCHAR (255) NULL,
    CONSTRAINT [PK__IMS__3214EC0713F1F5EB] PRIMARY KEY CLUSTERED ([Id] ASC)
);

