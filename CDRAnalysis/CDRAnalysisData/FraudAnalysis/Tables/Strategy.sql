CREATE TABLE [FraudAnalysis].[Strategy] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Description]     VARCHAR (255)  NULL,
    [UserId]          INT            NULL,
    [CreationDate]    DATETIME       CONSTRAINT [DF_Strategy_CreationDate] DEFAULT (getdate()) NULL,
    [Name]            VARCHAR (20)   NULL,
    [IsDefault]       BIT            CONSTRAINT [DF_Strategy_IsDefault] DEFAULT ((0)) NULL,
    [StrategyContent] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Strategy] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [uc_Strategy_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

