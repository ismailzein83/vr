CREATE TABLE [FraudAnalysis].[NumberProfile] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [SubscriberNumber] VARCHAR (30)   CONSTRAINT [DF__ts_Number__Subsc__6DB73E6A] DEFAULT (NULL) NULL,
    [FromDate]         DATETIME       CONSTRAINT [DF__ts_Number__FromD__6EAB62A3] DEFAULT (NULL) NULL,
    [ToDate]           DATETIME       CONSTRAINT [DF__ts_Number__ToDat__6F9F86DC] DEFAULT (NULL) NULL,
    [PeriodId]         INT            CONSTRAINT [DF__ts_Number__Perio__00CA12DE] DEFAULT (NULL) NULL,
    [StrategyId]       INT            NULL,
    [AggregateValues]  NVARCHAR (MAX) NULL,
    CONSTRAINT [PK__ts_Numbe__3214EC076BCEF5F8] PRIMARY KEY CLUSTERED ([Id] ASC)
);





