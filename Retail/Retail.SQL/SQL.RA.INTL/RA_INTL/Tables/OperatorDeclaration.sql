CREATE TABLE [RA_INTL].[OperatorDeclaration] (
    [ID]               BIGINT        IDENTITY (1, 1) NOT NULL,
    [OperatorID]       BIGINT        NULL,
    [CurrencyID]       INT           NULL,
    [CreatedTime]      DATETIME      NULL,
    [CreatedBy]        INT           NULL,
    [LastModifiedTime] DATETIME      NULL,
    [LastModifiedBy]   INT           NULL,
    [Services]         VARCHAR (MAX) NULL,
    [Period]           INT           NULL,
    [timestamp]        ROWVERSION    NULL,
    CONSTRAINT [PK__Operator__3214EC2702084FDA] PRIMARY KEY CLUSTERED ([ID] ASC)
);

