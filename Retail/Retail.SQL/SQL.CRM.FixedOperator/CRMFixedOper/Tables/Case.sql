CREATE TABLE [CRMFixedOper].[Case] (
    [ID]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [Description]       NVARCHAR (255)   NULL,
    [Attachment]        NVARCHAR (MAX)   NULL,
    [CreatedTime]       DATETIME         NULL,
    [CreatedBy]         INT              NULL,
    [LastModifiedTime]  DATETIME         NULL,
    [LastModifiedBy]    INT              NULL,
    [CustomerId]        BIGINT           NULL,
    [TicketCategory]    INT              NULL,
    [TicketSubCategory] INT              NULL,
    [TicketType]        INT              NULL,
    [Contract]          BIGINT           NULL,
    [Status]            UNIQUEIDENTIFIER NULL,
    [AssignedTo]        INT              NULL,
    [ProcessInstanceID] BIGINT           NULL,
    CONSTRAINT [PK__Case__3214EC274C6B5938] PRIMARY KEY CLUSTERED ([ID] ASC)
);

