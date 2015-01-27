CREATE TABLE [dbo].[Billing_CDR_Cost] (
    [ID]                 BIGINT          NOT NULL,
    [ZoneID]             INT             NOT NULL,
    [Net]                FLOAT (53)      NOT NULL,
    [CurrencyID]         VARCHAR (3)     NOT NULL,
    [RateValue]          FLOAT (53)      NOT NULL,
    [RateID]             BIGINT          NOT NULL,
    [Discount]           FLOAT (53)      NULL,
    [RateType]           TINYINT         CONSTRAINT [DF_Billing_CDR_Cost_RateType] DEFAULT ((0)) NOT NULL,
    [ToDConsiderationID] BIGINT          NULL,
    [FirstPeriod]        FLOAT (53)      NULL,
    [RepeatFirstperiod]  TINYINT         NULL,
    [FractionUnit]       TINYINT         NULL,
    [TariffID]           BIGINT          NULL,
    [CommissionValue]    FLOAT (53)      CONSTRAINT [DF__Billing.C__Commi__5C37ACAD] DEFAULT ((0)) NOT NULL,
    [CommissionID]       INT             NULL,
    [ExtraChargeValue]   FLOAT (53)      CONSTRAINT [DF__Billing.C__Extra__5D2BD0E6] DEFAULT ((0)) NOT NULL,
    [ExtraChargeID]      INT             NULL,
    [Updated]            SMALLDATETIME   NOT NULL,
    [DurationInSeconds]  NUMERIC (13, 4) NULL,
    [Code]               VARCHAR (20)    NULL,
    [Attempt]            DATETIME        NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [PK_Billing_CDR_Cost]
    ON [dbo].[Billing_CDR_Cost]([ID] ASC)
    ON [TOne_CDR];


GO
CREATE CLUSTERED INDEX [IX_Billing_CDR_Cost_Attempt]
    ON [dbo].[Billing_CDR_Cost]([Attempt] ASC)
    ON [TOne_CDR];


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'''N'' for Normal (Peak), ''O'' for Offpeak and ''W'' for Weekend Rate ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Billing_CDR_Cost', @level2type = N'COLUMN', @level2name = N'RateType';

