﻿CREATE TABLE [TOneWhS_BE].[SellingNumberPlan] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (255) NOT NULL,
    [timestamp] ROWVERSION     NULL,
    CONSTRAINT [PK_SellingNumberPlan] PRIMARY KEY CLUSTERED ([ID] ASC)
);

