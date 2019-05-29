CREATE TABLE [NIM_PSTN].[MDF] (
    [Id]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [Site]             BIGINT           NULL,
    [Status]           UNIQUEIDENTIFIER NULL,
    [Region]           INT              NULL,
    [City]             INT              NULL,
    [Town]             INT              NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK__MDF__3214EC075535A963] PRIMARY KEY CLUSTERED ([Id] ASC)
);



