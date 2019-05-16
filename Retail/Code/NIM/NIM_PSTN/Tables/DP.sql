CREATE TABLE [NIM_PSTN].[DP] (
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
    CONSTRAINT [PK__DP__3214EC0702FC7413] PRIMARY KEY CLUSTERED ([Id] ASC)
);

