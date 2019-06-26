CREATE TABLE [NIM_PSTN].[FDB] (
    [Id]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NULL,
    [Number]           NVARCHAR (255) NULL,
    [Site]             BIGINT         NULL,
    [Region]           INT            NULL,
    [City]             INT            NULL,
    [Town]             INT            NULL,
    [Street]           BIGINT         NULL,
    [BuildingDetails]  NVARCHAR (MAX) NULL,
    [Splitter]         BIGINT         NULL,
    [CreatedBy]        INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK__FDB__3214EC07531856C7] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_FDB_Name_Site] UNIQUE NONCLUSTERED ([Name] ASC, [Site] ASC),
    CONSTRAINT [IX_FDB_Number] UNIQUE NONCLUSTERED ([Number] ASC)
);



