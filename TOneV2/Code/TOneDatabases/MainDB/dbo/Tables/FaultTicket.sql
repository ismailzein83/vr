﻿CREATE TABLE [dbo].[FaultTicket] (
    [FaultTicketID]    INT           IDENTITY (1, 1) NOT NULL,
    [CarrierAccountID] VARCHAR (10)  NOT NULL,
    [ZoneID]           INT           NOT NULL,
    [TicketType]       TINYINT       NOT NULL,
    [Status]           TINYINT       NOT NULL,
    [TicketDate]       DATETIME      NOT NULL,
    [UpdateDate]       DATETIME      NOT NULL,
    [FromDate]         DATETIME      NOT NULL,
    [ToDate]           DATETIME      NULL,
    [Reference]        VARCHAR (255) NULL,
    [AlertPeriod]      BIGINT        NULL,
    [AlertTime]        DATETIME      NULL,
    [Attempts]         INT           NULL,
    [ASR]              FLOAT (53)    NULL,
    [ACD]              FLOAT (53)    NULL,
    [TestNumbers]      VARCHAR (MAX) NULL,
    [Attachment]       IMAGE         NULL,
    [OwnReference]     VARCHAR (50)  NULL,
    [FileName]         VARCHAR (50)  NULL,
    [UserID]           INT           NULL,
    CONSTRAINT [PK_FaultTicket] PRIMARY KEY CLUSTERED ([FaultTicketID] ASC)
);

