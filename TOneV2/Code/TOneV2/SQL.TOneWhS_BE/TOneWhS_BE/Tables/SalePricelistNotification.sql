CREATE TABLE [TOneWhS_BE].[SalePricelistNotification] (
    [ID]                INT        IDENTITY (1, 1) NOT NULL,
    [CustomerID]        INT        NOT NULL,
    [PricelistID]       INT        NOT NULL,
    [EmailCreationDate] DATETIME   NOT NULL,
    [FileID]            BIGINT     NOT NULL,
    [timestamp]         ROWVERSION NOT NULL,
    [CreatedTime]       DATETIME   CONSTRAINT [DF_SalePricelistNotification_CreatedTime] DEFAULT (getdate()) NOT NULL
);

