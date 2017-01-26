CREATE TABLE [Retail_BE].[DID] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Number]      VARCHAR (50)   NOT NULL,
    [Old_Number]  VARCHAR (50)   NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [timestamp]   ROWVERSION     NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_DID_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_DID] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_DID_Number] UNIQUE NONCLUSTERED ([Old_Number] ASC)
);



