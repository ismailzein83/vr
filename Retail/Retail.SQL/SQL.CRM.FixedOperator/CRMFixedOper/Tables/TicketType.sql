CREATE TABLE [CRMFixedOper].[TicketType] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [TicketCategory]    INT            NULL,
    [TicketSubCategory] INT            NULL,
    [Name]              NVARCHAR (255) NULL,
    [RoutedTo]          INT            NULL,
    [LateTickets]       INT            NULL,
    [EscalationLevel]   NVARCHAR (MAX) NULL,
    [CreatedTime]       DATETIME       NULL,
    [CreatedBy]         INT            NULL,
    [LastModifiedTime]  DATETIME       NULL,
    [LastModifiedBy]    INT            NULL,
    [timestamp]         ROWVERSION     NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

