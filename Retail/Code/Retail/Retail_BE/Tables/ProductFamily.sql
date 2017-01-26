CREATE TABLE [Retail_BE].[ProductFamily] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Settings]    NVARCHAR (MAX) NOT NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_ProductFamily_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL
);

