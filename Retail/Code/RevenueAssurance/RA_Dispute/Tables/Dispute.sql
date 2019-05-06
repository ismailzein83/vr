CREATE TABLE [RA_Dispute].[Dispute] (
    [ID]                    BIGINT           IDENTITY (1, 1) NOT NULL,
    [OperatorID]            INT              NULL,
    [PeriodID]              INT              NULL,
    [StatusID]              UNIQUEIDENTIFIER NULL,
    [ReferenceNumber]       NVARCHAR (255)   NULL,
    [Notes]                 NVARCHAR (1000)  NULL,
    [Attachments]           NVARCHAR (MAX)   NULL,
    [TrafficType]           INT              NULL,
    [TrafficDirection]      INT              NULL,
    [Scope]                 INT              NULL,
    [SendEmail]             INT              NULL,
    [CreatedBy]             INT              NULL,
    [CreatedTime]           DATETIME         CONSTRAINT [DF_Dispute_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]        INT              NULL,
    [LastModifiedTime]      DATETIME         NULL,
    [DeclaredRevenue]       DECIMAL (22, 8)  NULL,
    [MeasuredRevenue]       DECIMAL (22, 8)  NULL,
    [RevenueTolerance]      DECIMAL (22, 8)  NULL,
    [DeclaredNbOfSMS]       BIGINT           NULL,
    [MeasuredNbOfSMS]       INT              NULL,
    [SMSTolerance]          DECIMAL (20, 8)  NULL,
    [DeclaredNumberOfCalls] BIGINT           NULL,
    [MeasuredNbOfCalls]     INT              NULL,
    [CallsTolerance]        DECIMAL (22, 8)  NULL,
    [DeclaredDuration]      DECIMAL (20, 8)  NULL,
    [MeasuredDuration]      DECIMAL (20, 8)  NULL,
    [DurationTolerance]     DECIMAL (22, 8)  NULL,
    CONSTRAINT [PK__Dispute__3214EC277E37BEF6] PRIMARY KEY CLUSTERED ([ID] ASC)
);



