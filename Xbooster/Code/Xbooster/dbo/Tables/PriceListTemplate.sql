CREATE TABLE [dbo].[PriceListTemplate] (
    [ID]            INT            IDENTITY (1, 1) NOT NULL,
    [Name]          VARCHAR (255)  NULL,
    [UserID]        INT            NULL,
    [Type]          VARCHAR (255)  NULL,
    [ConfigDetails] NVARCHAR (MAX) NULL,
    [CreatedTime]   DATETIME       CONSTRAINT [DF_PriceListTemplate_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]     ROWVERSION     NOT NULL,
    CONSTRAINT [PK_PriceListTemplate] PRIMARY KEY CLUSTERED ([ID] ASC)
);

