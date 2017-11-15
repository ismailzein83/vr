﻿CREATE TABLE [TOneWhS_Sales].[RP_SaleRate_Changed] (
    [ID]                BIGINT   NOT NULL,
    [ProcessInstanceID] BIGINT   NOT NULL,
    [EED]               DATETIME NULL
);




GO
CREATE CLUSTERED INDEX [IX_RP_SaleRate_Changed_ProcessInstanceID]
    ON [TOneWhS_Sales].[RP_SaleRate_Changed]([ProcessInstanceID] ASC);

