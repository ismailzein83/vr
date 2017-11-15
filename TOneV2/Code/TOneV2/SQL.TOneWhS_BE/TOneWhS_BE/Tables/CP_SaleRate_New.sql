﻿CREATE TABLE [TOneWhS_BE].[CP_SaleRate_New] (
    [ID]                BIGINT          NULL,
    [ProcessInstanceID] BIGINT          NOT NULL,
    [ZoneID]            BIGINT          NOT NULL,
    [PriceListID]       INT             NOT NULL,
    [CurrencyID]        INT             NULL,
    [NormalRate]        DECIMAL (20, 8) NOT NULL,
    [BED]               DATETIME        NOT NULL,
    [EED]               DATETIME        NULL
);




GO
CREATE CLUSTERED INDEX [IX_CP_SaleRate_New_ProcessInstanceID]
    ON [TOneWhS_BE].[CP_SaleRate_New]([ProcessInstanceID] ASC);

