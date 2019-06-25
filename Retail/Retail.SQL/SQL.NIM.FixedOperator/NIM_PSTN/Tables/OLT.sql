CREATE TABLE [NIM_PSTN].[OLT] (
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
    [IMS]              BIGINT         NULL,
    CONSTRAINT [PK__OLT__3214EC0740058253] PRIMARY KEY CLUSTERED ([Id] ASC)
);

