CREATE TABLE [CRMFixedOper].[EscalationLevel] (
    [UserId]           INT              NULL,
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [CompletedTime]    DATETIME         NULL,
    [TicketId]         BIGINT           NULL,
    [Status]           UNIQUEIDENTIFIER NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL
);

