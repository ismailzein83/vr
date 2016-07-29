﻿CREATE TABLE [TOneWhS_Sales].[RP_SaleRate_Preview] (
    [ZoneName]               NVARCHAR (255) NOT NULL,
    [ProcessInstanceID]      BIGINT         NOT NULL,
    [CurrentRate]            DECIMAL (9, 5) NULL,
    [IsCurrentRateInherited] BIT            NULL,
    [NewRate]                DECIMAL (9, 5) NULL,
    [ChangeType]             INT            NOT NULL,
    [EffectiveOn]            DATETIME       NOT NULL,
    [EffectiveUntil]         DATETIME       NULL
);



