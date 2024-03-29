﻿CREATE TYPE [Retail_EDR].[RingoEventType] AS TABLE (
    [MSISDN]                 NVARCHAR (100)   NULL,
    [EventIdMvno]            INT              NULL,
    [EventId]                INT              NULL,
    [Event]                  VARCHAR (100)    NULL,
    [Parameters]             VARCHAR (500)    NULL,
    [CreatedDate]            DATETIME         NULL,
    [AccountId]              BIGINT           NULL,
    [FileName]               VARCHAR (200)    NULL,
    [PromotionCode]          VARCHAR (200)    NULL,
    [PromotionId]            INT              NULL,
    [ActivationDate]         DATETIME         NULL,
    [CustomerActivationDate] DATETIME         NULL,
    [PackagePrice]           DECIMAL (20, 10) NULL,
    [Amount]                 INT              NULL,
    [Balance]                DECIMAL (20, 10) NULL);











