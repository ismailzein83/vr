CREATE TABLE [NIM_PSTN].[Splitter] (
    [Id]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NULL,
    [Site]             BIGINT         NULL,
    [Region]           INT            NULL,
    [City]             INT            NULL,
    [Town]             INT            NULL,
    [CreatedBy]        INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [timestamp]        ROWVERSION     NULL,
    [OLT]              BIGINT         NULL,
    CONSTRAINT [PK__Splitter__3214EC070880433F] PRIMARY KEY CLUSTERED ([Id] ASC)
);

