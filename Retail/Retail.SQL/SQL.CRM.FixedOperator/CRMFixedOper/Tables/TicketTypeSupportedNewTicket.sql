CREATE TABLE [CRMFixedOper].[TicketTypeSupportedNewTicket] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [TicketTypeID]     UNIQUEIDENTIFIER NULL,
    [NewTicketTypeID]  UNIQUEIDENTIFIER NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

