CREATE TABLE [dbo].[CarrierCode] (
    [Id]                 INT          NOT NULL,
    [Code]               VARCHAR (20) NULL,
    [ZoneId]             INT          NULL,
    [BeginEffectiveDate] DATETIME     NULL,
    [EndEffectiveDate]   DATETIME     NULL,
    [PriceListId]        INT          NULL,
    [ClosingId]          INT          NULL,
    [Blocked]            TINYINT      NULL,
    [oldid]              INT          NULL,
    [Status]             TINYINT      NULL,
    [OldStatus]          TINYINT      NULL,
    [Processed]          TINYINT      NULL
);

