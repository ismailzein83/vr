﻿CREATE TABLE [CRMFixedOper].[TicketSubCategory] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [CategoryID]       UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NULL,
    [Faults]           INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_TicketSubCategory] PRIMARY KEY CLUSTERED ([ID] ASC)
);



