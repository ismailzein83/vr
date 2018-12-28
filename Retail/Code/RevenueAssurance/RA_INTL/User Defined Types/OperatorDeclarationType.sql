CREATE TYPE [RA_INTL].[OperatorDeclarationType] AS TABLE (
    [ID]               BIGINT        NULL,
    [OperatorID]       BIGINT        NULL,
    [CreatedTime]      DATETIME      NULL,
    [CreatedBy]        INT           NULL,
    [LastModifiedTime] DATETIME      NULL,
    [LastModifiedBy]   INT           NULL,
    [Services]         VARCHAR (MAX) NULL,
    [Period]           INT           NULL,
    [CurrencyID]       INT           NULL);

