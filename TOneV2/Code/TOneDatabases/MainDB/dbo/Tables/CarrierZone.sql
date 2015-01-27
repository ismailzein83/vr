CREATE TABLE [dbo].[CarrierZone] (
    [Id]                 INT           NOT NULL,
    [CarrierId]          INT           NULL,
    [CountryName]        VARCHAR (200) NULL,
    [BeginEffectiveDate] DATETIME      NULL,
    [EndEffectiveDate]   DATETIME      NULL,
    [PriceListId]        INT           NULL,
    [ClosingId]          INT           NULL,
    [Blocked]            TINYINT       NULL,
    [Other]              INT           NULL,
    [Status]             TINYINT       NULL,
    [LCRId]              INT           NULL
);

