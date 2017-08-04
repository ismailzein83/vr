CREATE TABLE [FraudAnalysis].[Strategy] (
    [ID]            INT            IDENTITY (1, 1) NOT NULL,
    [Name]          VARCHAR (20)   NOT NULL,
    [Description]   VARCHAR (255)  NULL,
    [UserID]        INT            NOT NULL,
    [LastUpdatedOn] DATETIME       CONSTRAINT [DF_Strategy_CreationDate] DEFAULT (getdate()) NOT NULL,
    [Settings]      NVARCHAR (MAX) NOT NULL,
    [timestamp]     ROWVERSION     NULL,
    CONSTRAINT [PK_Strategy] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [uc_Strategy_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);





