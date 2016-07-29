CREATE TABLE [Retail_BE].[CreditClass] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Settings]    NVARCHAR (MAX) NOT NULL,
    [timestamp]   ROWVERSION     NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_CreditClass_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_CreditClass] PRIMARY KEY CLUSTERED ([ID] ASC)
);

