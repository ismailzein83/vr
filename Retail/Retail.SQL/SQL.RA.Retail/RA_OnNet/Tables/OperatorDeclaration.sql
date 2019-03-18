CREATE TABLE [RA_OnNet].[OperatorDeclaration] (
    [ID]               BIGINT        IDENTITY (1, 1) NOT NULL,
    [OperatorId]       BIGINT        NULL,
    [CreatedTime]      DATETIME      NULL,
    [CreatedBy]        INT           NULL,
    [LastModifiedTime] DATETIME      NULL,
    [LastModifiedBy]   INT           NULL,
    [Services]         VARCHAR (MAX) NULL,
    [Period]           INT           NULL,
    [CurrencyID]       INT           NULL,
    [timestamp]        ROWVERSION    NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

