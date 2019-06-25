CREATE TABLE [NIM_PSTN].[SwitchDevice] (
    [Id]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [Slot]             BIGINT           NULL,
    [Type]             BIGINT           NULL,
    [Status]           UNIQUEIDENTIFIER NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK__Device__3214EC0749C3F6B7] PRIMARY KEY CLUSTERED ([Id] ASC)
);

