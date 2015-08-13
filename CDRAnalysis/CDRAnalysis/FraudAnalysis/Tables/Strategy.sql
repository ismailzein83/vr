CREATE TABLE [FraudAnalysis].[Strategy] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Name]            VARCHAR (20)   NOT NULL,
    [Description]     VARCHAR (255)  NULL,
    [UserId]          INT            NOT NULL,
    [LastUpdatedOn]   DATETIME       CONSTRAINT [DF_Strategy_CreationDate] DEFAULT (getdate()) NOT NULL,
    [IsDefault]       BIT            CONSTRAINT [DF_Strategy_IsDefault] DEFAULT ((0)) NOT NULL,
    [IsEnabled]       BIT            CONSTRAINT [DF_Strategy_IsDefault1] DEFAULT ((0)) NOT NULL,
    [PeriodId]        INT            NOT NULL,
    [StrategyContent] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Strategy] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [uc_Strategy_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

