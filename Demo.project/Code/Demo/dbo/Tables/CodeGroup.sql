﻿CREATE TABLE [dbo].[CodeGroup] (
    [ID]        INT          IDENTITY (1, 1) NOT NULL,
    [CountryID] INT          NOT NULL,
    [Code]      VARCHAR (20) NOT NULL,
    [timestamp] ROWVERSION   NULL,
    CONSTRAINT [PK_CodeGroup_1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_CodeGroup_Code] UNIQUE NONCLUSTERED ([Code] ASC)
);

