CREATE TABLE [CRMFixedOper].[Ticket] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [TicketTypeID]     UNIQUEIDENTIFIER NULL,
    [CaseId]           BIGINT           NULL,
    [ReferenceNumber]  NVARCHAR (255)   NULL,
    [Customer]         BIGINT           NULL,
    [Order]            BIGINT           NULL,
    [Title]            NVARCHAR (255)   NULL,
    [AssignedGroup]    INT              NULL,
    [StatusID]         UNIQUEIDENTIFIER NULL,
    [StartedBy]        INT              NULL,
    [CompletedBy]      INT              NULL,
    [StartedTime]      DATETIME         NULL,
    [CompletedTime]    DATETIME         NULL,
    [EscalationLevel]  INT              NULL,
    [CustomFields]     NVARCHAR (MAX)   NULL,
    [TaskId]           BIGINT           NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [Attachments]      NVARCHAR (MAX)   NULL,
    [Area]             BIGINT           NULL,
    [FreeNotes]        VARCHAR (255)    NULL,
    [AreaID]           BIGINT           NULL,
    [SiteID]           BIGINT           NULL,
    CONSTRAINT [PK__Ticket__3214EC2755F4C372] PRIMARY KEY CLUSTERED ([ID] ASC)
);







