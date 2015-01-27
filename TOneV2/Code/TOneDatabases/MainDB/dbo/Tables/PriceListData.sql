CREATE TABLE [dbo].[PriceListData] (
    [PriceListID]     INT        NOT NULL,
    [SourceFileBytes] IMAGE      NULL,
    [timestamp]       ROWVERSION NULL,
    [DS_ID_auto]      INT        IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_PriceListData] PRIMARY KEY CLUSTERED ([PriceListID] ASC)
);

