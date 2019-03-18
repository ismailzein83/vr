CREATE TABLE [RA_ICX].[OperatorDeclaration] (
    [ID]               BIGINT        IDENTITY (1, 1) NOT NULL,
    [OperatorID]       BIGINT        NULL,
    [Period]           INT           NULL,
    [Services]         VARCHAR (MAX) NULL,
    [CurrencyID]       INT           NULL,
    [CreatedTime]      DATETIME      NULL,
    [CreatedBy]        INT           NULL,
    [LastModifiedTime] DATETIME      NULL,
    [LastModifiedBy]   INT           NULL,
    [timestamp]        ROWVERSION    NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

