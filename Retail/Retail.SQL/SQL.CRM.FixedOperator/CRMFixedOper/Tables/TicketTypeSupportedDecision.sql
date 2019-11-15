CREATE TABLE [CRMFixedOper].[TicketTypeSupportedDecision] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [TicketTypeID]     UNIQUEIDENTIFIER NULL,
    [DecisionID]       UNIQUEIDENTIFIER NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK__TicketTy__3214EC274C364F0E] PRIMARY KEY CLUSTERED ([ID] ASC)
);

