﻿CREATE TABLE [TOneWhS_SMSAnalytics].[SMSTrafficStats15Min] (
    [Id]                           BIGINT           NULL,
    [BatchStart]                   DATETIME         NULL,
    [SwitchID]                     INT              NULL,
    [CustomerID]                   INT              NULL,
    [CustomerProfileId]            INT              NULL,
    [SupplierId]                   INT              NULL,
    [SupplierProfileId]            INT              NULL,
    [OriginationMC_Id]             INT              NULL,
    [OriginationMN_Id]             INT              NULL,
    [DestinationMC_Id]             INT              NULL,
    [DestinationMN_Id]             INT              NULL,
    [PortIN]                       VARCHAR (42)     NULL,
    [PortOUT]                      VARCHAR (42)     NULL,
    [FirstSMSSent]                 DATETIME         NULL,
    [LastSMSSent]                  DATETIME         NULL,
    [NumberOfSMS]                  INT              NULL,
    [NumberOfDeliveredSMS]         INT              NULL,
    [NumberOfCustomerDeliveredSMS] INT              NULL,
    [NumberOfSupplierDeliveredSMS] INT              NULL,
    [SumOfDeliveryDelayInSeconds]  DECIMAL (20, 4)  NULL,
    [PricedSMS]                    INT              NULL,
    [SaleNet]                      DECIMAL (26, 10) NULL,
    [SaleCurrencyId]               INT              NULL,
    [CostNet]                      DECIMAL (26, 10) NULL,
    [CostCurrencyId]               INT              NULL,
    [SaleFinancialAccountId]       INT              NULL,
    [CostFinancialAccountId]       INT              NULL,
    [Type]                         INT              NULL,
    CONSTRAINT [IX_SMSTrafficStats15Min_Id] UNIQUE NONCLUSTERED ([Id] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_SMSTrafficStats15Min_BatchStart]
    ON [TOneWhS_SMSAnalytics].[SMSTrafficStats15Min]([BatchStart] ASC);

