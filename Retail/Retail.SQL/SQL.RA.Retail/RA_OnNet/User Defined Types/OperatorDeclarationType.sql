CREATE TYPE [RA_OnNet].[OperatorDeclarationType] AS TABLE (
    [ID]               BIGINT        NULL,
    [OperatorId]       BIGINT        NULL,
    [CreatedTime]      DATETIME      NULL,
    [CreatedBy]        INT           NULL,
    [LastModifiedTime] DATETIME      NULL,
    [LastModifiedBy]   INT           NULL,
    [Services]         VARCHAR (MAX) NULL,
    [Period]           INT           NULL,
    [CurrencyID]       INT           NULL);

