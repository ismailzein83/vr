CREATE TABLE [TOneWhS_BE].[CustomerCountry] (
    [ID]         INT        NOT NULL,
    [CustomerID] INT        NOT NULL,
    [CountryID]  INT        NOT NULL,
    [BED]        DATETIME   NOT NULL,
    [EED]        DATETIME   NULL,
    [timestamp]  ROWVERSION NULL,
    CONSTRAINT [PK_CustomerCountry] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_CustomerCountry_CarrierAccount] FOREIGN KEY ([CustomerID]) REFERENCES [TOneWhS_BE].[CarrierAccount] ([ID])
);

