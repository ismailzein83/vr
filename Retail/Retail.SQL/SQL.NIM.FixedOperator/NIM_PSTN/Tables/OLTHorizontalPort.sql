CREATE TABLE [NIM_PSTN].[OLTHorizontalPort] (
    [Id]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [OLTHorizontal]    BIGINT           NULL,
    [Status]           UNIQUEIDENTIFIER NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK__OLTHoriz__3214EC072F9A1060] PRIMARY KEY CLUSTERED ([Id] ASC)
);

