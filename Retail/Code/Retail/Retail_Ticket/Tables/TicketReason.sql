CREATE TABLE [Retail_Ticket].[TicketReason] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_TicketReason_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_TicketReason] PRIMARY KEY CLUSTERED ([ID] ASC)
);

