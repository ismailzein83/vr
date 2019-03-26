CREATE TABLE [TCAnal_CDR].[CalledNumberMapping] (
    [ID]               BIGINT        IDENTITY (1, 1) NOT NULL,
    [OperatorID]       BIGINT        NULL,
    [Number]           NVARCHAR (40) NULL,
    [MappedNumber]     NVARCHAR (40) NULL,
    [CreatedTime]      DATETIME      NULL,
    [LastModifiedTime] DATETIME      NULL,
    [CreatedBy]        INT           NULL,
    [LastModifiedBy]   INT           NULL,
    [timestamp]        ROWVERSION    NULL,
    CONSTRAINT [PK_CalledNumberMapping] PRIMARY KEY CLUSTERED ([ID] ASC)
);

