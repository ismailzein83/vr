﻿CREATE TABLE [dbo].[GeneratedCalls] (
    [ID]                         INT            IDENTITY (1, 1) NOT NULL,
    [SourceID]                   INT            NOT NULL,
    [MobileOperatorID]           INT            NULL,
    [StatusID]                   INT            NOT NULL,
    [PriorityID]                 INT            NULL,
    [ReportingStatusID]          INT            NOT NULL,
    [DurationInSeconds]          INT            NOT NULL,
    [MobileOperatorFeedbackID]   INT            NULL,
    [a_number]                   VARCHAR (100)  NULL,
    [b_number]                   VARCHAR (100)  NOT NULL,
    [CLI]                        VARCHAR (100)  NULL,
    [OriginationNetwork]         VARCHAR (100)  NULL,
    [AssignedTo]                 INT            NULL,
    [AssignedBy]                 INT            NULL,
    [ReportID]                   INT            NULL,
    [AttemptDateTime]            DATETIME2 (0)  NOT NULL,
    [LevelOneComparisonDateTime] DATETIME2 (0)  NULL,
    [LevelTwoComparisonDateTime] DATETIME2 (0)  NULL,
    [FeedbackDateTime]           DATETIME2 (0)  NULL,
    [AssignmentDateTime]         DATETIME2 (0)  NULL,
    [ImportID]                   INT            NULL,
    [ReportingStatusChangedBy]   INT            NULL,
    [Level1Comparison]           BIT            NULL,
    [Level2Comparison]           BIT            NULL,
    [ToneFeedbackID]             INT            NULL,
    [FeedbackNotes]              VARCHAR (5000) NULL,
    [Carrier]                    VARCHAR (100)  NULL,
    [Reference]                  VARCHAR (50)   NULL,
    [Type]                       VARCHAR (5)    NULL,
    [ReportingStatusSecurityID]  INT            NULL,
    [ReportSecID]                INT            NULL
);

