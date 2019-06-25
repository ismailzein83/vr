CREATE TABLE [NIM_PSTN].[OLTVerticalPort] (
    [Id]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [OLTVertical]      BIGINT           NULL,
    [Status]           UNIQUEIDENTIFIER NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK__OLTPort__3214EC0743D61337] PRIMARY KEY CLUSTERED ([Id] ASC)
);

