CREATE TABLE [dbo].[RecievedCalls] (
    [ID]                 INT           IDENTITY (1, 1) NOT NULL,
    [SourceID]           INT           NOT NULL,
    [MobileOperatorID]   INT           NULL,
    [DurationInSeconds]  INT           NOT NULL,
    [a_number]           VARCHAR (100) NULL,
    [b_number]           VARCHAR (100) NOT NULL,
    [CLI]                VARCHAR (100) NULL,
    [AttemptDateTime]    DATETIME2 (0) NOT NULL,
    [ImportID]           INT           NOT NULL,
    [Carrier]            VARCHAR (100) NULL,
    [ClientID]           INT           NULL,
    [GeneratedCallID]    INT           NULL,
    [OriginationNetwork] VARCHAR (100) NULL,
    [Reference]          VARCHAR (50)  NULL,
    [Type]               VARCHAR (5)   NULL
);

