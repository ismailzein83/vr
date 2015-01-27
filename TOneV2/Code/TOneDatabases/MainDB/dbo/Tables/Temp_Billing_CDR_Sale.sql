CREATE TABLE [dbo].[Temp_Billing_CDR_Sale] (
    [ID]                 BIGINT          NOT NULL,
    [ZoneID]             INT             NOT NULL,
    [Net]                FLOAT (53)      NOT NULL,
    [CurrencyID]         VARCHAR (3)     NOT NULL,
    [RateValue]          FLOAT (53)      NOT NULL,
    [RateID]             BIGINT          NOT NULL,
    [Discount]           FLOAT (53)      NULL,
    [RateType]           TINYINT         NOT NULL,
    [ToDConsiderationID] BIGINT          NULL,
    [FirstPeriod]        FLOAT (53)      NULL,
    [RepeatFirstperiod]  TINYINT         NULL,
    [FractionUnit]       TINYINT         NULL,
    [TariffID]           BIGINT          NULL,
    [CommissionValue]    FLOAT (53)      NOT NULL,
    [CommissionID]       INT             NULL,
    [ExtraChargeValue]   FLOAT (53)      NOT NULL,
    [ExtraChargeID]      INT             NULL,
    [Updated]            SMALLDATETIME   NOT NULL,
    [DurationInSeconds]  NUMERIC (13, 4) NULL,
    [Code]               VARCHAR (20)    NULL,
    [Attempt]            DATETIME        NULL
);

