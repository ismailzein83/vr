﻿CREATE TABLE [TOneWhS_BE].[CustomerZone] (
    [ID]               INT           IDENTITY (1, 1) NOT NULL,
    [CustomerID]       INT           NOT NULL,
    [Details]          VARCHAR (MAX) NOT NULL,
    [BED]              DATETIME      NOT NULL,
    [timestamp]        ROWVERSION    NULL,
    [LastModifiedTime] DATETIME      CONSTRAINT [DF_CustomerZone_LastModifiedTime] DEFAULT (getdate()) NULL,
    [CreatedTime]      DATETIME      CONSTRAINT [DF_CustomerZone_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_CustomerZones] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_CustomerZones_CarrierAccount] FOREIGN KEY ([CustomerID]) REFERENCES [TOneWhS_BE].[CarrierAccount] ([ID])
);





