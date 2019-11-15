CREATE TABLE [CRMFixedOper].[Case] (
    [ID]                  BIGINT           IDENTITY (1, 1) NOT NULL,
    [Description]         NVARCHAR (255)   NULL,
    [Attachment]          NVARCHAR (MAX)   NULL,
    [CustomerId]          BIGINT           NULL,
    [Contract]            BIGINT           NULL,
    [Status]              UNIQUEIDENTIFIER NULL,
    [TicketCategoryID]    UNIQUEIDENTIFIER NULL,
    [TicketSubCategoryID] UNIQUEIDENTIFIER NULL,
    [TicketTypeID]        UNIQUEIDENTIFIER NULL,
    [AssignedTo]          INT              NULL,
    [ProcessInstanceID]   BIGINT           NULL,
    [CreatedTime]         DATETIME         NULL,
    [CreatedBy]           INT              NULL,
    [LastModifiedTime]    DATETIME         NULL,
    [LastModifiedBy]      INT              NULL,
    CONSTRAINT [PK__Case__3214EC274C6B5938] PRIMARY KEY CLUSTERED ([ID] ASC)
);



