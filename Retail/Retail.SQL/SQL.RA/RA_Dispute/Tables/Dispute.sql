﻿CREATE TABLE [RA_Dispute].[Dispute] (
    [ID]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [OperatorID]        INT              NULL,
    [PeriodID]          INT              NULL,
    [StatusID]          UNIQUEIDENTIFIER NULL,
    [ReferenceNumber]   NVARCHAR (255)   NULL,
    [Notes]             NVARCHAR (1000)  NULL,
    [Attachments]       NVARCHAR (MAX)   NULL,
    [TrafficType]       INT              NULL,
    [TrafficDirection]  INT              NULL,
    [Scope]             INT              NULL,
    [SendEmail]         INT              NULL,
    [CreatedBy]         INT              NULL,
    [CreatedTime]       DATETIME         CONSTRAINT [DF_Dispute_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]    INT              NULL,
    [LastModifiedTime]  DATETIME         NULL,
    [Revenue]           DECIMAL (22, 8)  NULL,
    [RevenueTolerance]  DECIMAL (22, 8)  NULL,
    [NbOfSMS]           BIGINT           NULL,
    [SMSTolerance]      DECIMAL (20, 8)  NULL,
    [NumberOfCalls]     BIGINT           NULL,
    [CallsTolerance]    DECIMAL (22, 8)  NULL,
    [Duration]          DECIMAL (20, 8)  NULL,
    [DurationTolerance] DECIMAL (22, 8)  NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);



