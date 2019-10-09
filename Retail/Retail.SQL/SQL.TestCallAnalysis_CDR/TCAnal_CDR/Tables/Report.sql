CREATE TABLE [TCAnal_CDR].[Report] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [OperatorId]       BIGINT         NULL,
    [Type]             INT            NULL,
    [FileIds]          NVARCHAR (MAX) NULL,
    [SentTime]         DATETIME       NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    CONSTRAINT [PK__Report__3214EC076FE99F9F] PRIMARY KEY CLUSTERED ([Id] ASC)
);



