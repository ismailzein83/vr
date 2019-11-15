CREATE TABLE [CRMFixedOper].[TicketType] (
    [ID]                  UNIQUEIDENTIFIER NOT NULL,
    [TicketSubCategoryID] UNIQUEIDENTIFIER NULL,
    [Name]                NVARCHAR (255)   NULL,
    [RoutedTo]            INT              NULL,
    [LateTickets]         INT              NULL,
    [EscalationLevel]     NVARCHAR (MAX)   NULL,
    [CreatedTime]         DATETIME         NULL,
    [CreatedBy]           INT              NULL,
    [LastModifiedTime]    DATETIME         NULL,
    [LastModifiedBy]      INT              NULL,
    [timestamp]           ROWVERSION       NULL,
    [InformationToShow]   INT              NULL,
    CONSTRAINT [PK_TicketType] PRIMARY KEY CLUSTERED ([ID] ASC)
);





