CREATE TABLE [TOneWhS_BE].[CustomerZone_Temp] (
    [ID]         INT           IDENTITY (1, 1) NOT NULL,
    [CustomerID] INT           NOT NULL,
    [Details]    VARCHAR (MAX) NOT NULL,
    [BED]        DATETIME      NOT NULL,
    [timestamp]  ROWVERSION    NULL
);

