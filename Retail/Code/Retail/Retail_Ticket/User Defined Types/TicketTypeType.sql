﻿CREATE TYPE [Retail_Ticket].[TicketTypeType] AS TABLE (
    [ID]               UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL);

