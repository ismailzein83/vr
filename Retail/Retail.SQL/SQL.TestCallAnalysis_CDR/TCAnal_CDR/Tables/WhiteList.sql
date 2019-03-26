CREATE TABLE [TCAnal_CDR].[WhiteList] (
    [ID]               BIGINT        IDENTITY (1, 1) NOT NULL,
    [OperatorId]       BIGINT        NULL,
    [Number]           NVARCHAR (40) NULL,
    [CreatedTime]      DATETIME      NULL,
    [LastModifiedTime] DATETIME      NULL,
    [CreatedBy]        INT           NULL,
    [LastModifiedBy]   INT           NULL,
    [timestamp]        ROWVERSION    NULL,
    CONSTRAINT [PK__WhiteLis__3214EC2732E0915F] PRIMARY KEY CLUSTERED ([ID] ASC)
);

