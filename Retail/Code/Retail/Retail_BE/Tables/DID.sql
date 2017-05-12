CREATE TABLE [Retail_BE].[DID] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [SourceID]    NVARCHAR (50)  NULL,
    [timestamp]   ROWVERSION     NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_DID_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_DID] PRIMARY KEY CLUSTERED ([ID] ASC)
);









