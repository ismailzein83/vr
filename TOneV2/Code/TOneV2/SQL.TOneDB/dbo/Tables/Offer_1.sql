CREATE TABLE [dbo].[Offer] (
    [OfferID]       INT           IDENTITY (1, 1) NOT NULL,
    [DealID]        INT           NOT NULL,
    [Zones]         VARCHAR (100) NULL,
    [Description]   NVARCHAR (50) NULL,
    [IsRetroActive] CHAR (1)      NULL,
    [IsPercentage]  CHAR (1)      NULL,
    [timestamp]     ROWVERSION    NOT NULL,
    CONSTRAINT [PK_Offer] PRIMARY KEY CLUSTERED ([OfferID] ASC)
);

