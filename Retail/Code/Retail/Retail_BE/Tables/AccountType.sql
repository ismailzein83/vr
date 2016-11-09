CREATE TABLE [Retail_BE].[AccountType] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [OldID]       INT              NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [Title]       NVARCHAR (255)   NOT NULL,
    [Settings]    NVARCHAR (MAX)   NOT NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_AccountType_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_AccountType] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_AccountType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);



