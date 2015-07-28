CREATE TABLE [FraudAnalysis].[SubscriberThreshold] (
    [Id]               INT             IDENTITY (1, 1) NOT NULL,
    [DateDay]          DATETIME        NULL,
    [SubscriberNumber] VARCHAR (50)    NULL,
    [Criteria1]        DECIMAL (18, 5) NULL,
    [Criteria2]        DECIMAL (18, 5) NULL,
    [Criteria3]        DECIMAL (18, 5) NULL,
    [Criteria4]        DECIMAL (18, 5) NULL,
    [Criteria5]        DECIMAL (18, 5) NULL,
    [Criteria6]        DECIMAL (18, 5) NULL,
    [Criteria7]        DECIMAL (18, 5) NULL,
    [Criteria8]        DECIMAL (18, 5) NULL,
    [Criteria9]        DECIMAL (18, 5) NULL,
    [Criteria10]       DECIMAL (18, 5) NULL,
    [Criteria11]       DECIMAL (18, 5) NULL,
    [Criteria12]       DECIMAL (18, 5) NULL,
    [Criteria13]       DECIMAL (18, 5) NULL,
    [Criteria14]       DECIMAL (18, 5) NULL,
    [Criteria15]       DECIMAL (18, 5) NULL,
    [Criteria16]       DECIMAL (18, 5) NULL,
    [Criteria17]       DECIMAL (18, 5) NULL,
    [Criteria18]       DECIMAL (18, 5) NULL,
    [SuspicionLevelId] INT             NULL,
    [StrategyId]       INT             NULL,
    CONSTRAINT [PK_SubscriberThreshold] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SubscriberThreshold_Suspicion_Level] FOREIGN KEY ([SuspicionLevelId]) REFERENCES [FraudAnalysis].[Suspicion_Level] ([Id])
);



