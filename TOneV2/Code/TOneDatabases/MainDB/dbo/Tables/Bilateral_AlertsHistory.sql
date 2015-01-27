CREATE TABLE [dbo].[Bilateral_AlertsHistory] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [CreatedDate] DATETIME       CONSTRAINT [DF_Bilateral_AlertsHistory_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [DealID]      INT            NOT NULL,
    [CarrierID]   CHAR (5)       NOT NULL,
    [AlertID]     INT            NOT NULL,
    [Tag]         NVARCHAR (250) NULL,
    [Description] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Bilateral_AlertsHistory] PRIMARY KEY CLUSTERED ([ID] ASC)
);

