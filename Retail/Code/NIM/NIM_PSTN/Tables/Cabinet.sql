CREATE TABLE [NIM_PSTN].[Cabinet] (
    [Id]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [Site]             BIGINT           NULL,
    [Location]         NVARCHAR (255)   NULL,
    [Status]           UNIQUEIDENTIFIER NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK__Cabinet__3214EC076A30C649] PRIMARY KEY CLUSTERED ([Id] ASC)
);

