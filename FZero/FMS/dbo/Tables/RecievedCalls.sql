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
    [ClientID]           INT           CONSTRAINT [DF_RecievedCalls_ClientID_1] DEFAULT ((1)) NULL,
    [GeneratedCallID]    INT           NULL,
    [OriginationNetwork] VARCHAR (100) NULL,
    [Reference]          VARCHAR (50)  NULL,
    [Type]               VARCHAR (5)   NULL,
    CONSTRAINT [PK_RecievedCalls] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 85),
    CONSTRAINT [FK_RecievedCalls_Clients] FOREIGN KEY ([ClientID]) REFERENCES [dbo].[Clients] ([ID]),
    CONSTRAINT [FK_RecievedCalls_Imports] FOREIGN KEY ([ImportID]) REFERENCES [dbo].[Imports] ([ID]),
    CONSTRAINT [FK_RecievedCalls_MobileOperators] FOREIGN KEY ([MobileOperatorID]) REFERENCES [dbo].[MobileOperators] ([ID]),
    CONSTRAINT [FK_RecievedCalls_Sources] FOREIGN KEY ([SourceID]) REFERENCES [dbo].[Sources] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [I_ClientID]
    ON [dbo].[RecievedCalls]([ClientID] ASC) WITH (FILLFACTOR = 85);


GO
CREATE NONCLUSTERED INDEX [I_GeneratedCallsID]
    ON [dbo].[RecievedCalls]([GeneratedCallID] ASC) WITH (FILLFACTOR = 85);


GO
CREATE NONCLUSTERED INDEX [I_AttemptDateTime]
    ON [dbo].[RecievedCalls]([AttemptDateTime] ASC) WITH (FILLFACTOR = 85);

