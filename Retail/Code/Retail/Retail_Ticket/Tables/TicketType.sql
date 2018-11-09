CREATE TABLE [Retail_Ticket].[TicketType] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_TicketType_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_TicketType] PRIMARY KEY CLUSTERED ([ID] ASC)
);

