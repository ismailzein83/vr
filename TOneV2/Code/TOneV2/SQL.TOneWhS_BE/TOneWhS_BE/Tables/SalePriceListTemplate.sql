CREATE TABLE [TOneWhS_BE].[SalePriceListTemplate] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Settings]    NVARCHAR (MAX) NOT NULL,
    [timestamp]   ROWVERSION     NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_SalePriceListTemplate_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_SalePriceListTemplate] PRIMARY KEY CLUSTERED ([ID] ASC)
);



