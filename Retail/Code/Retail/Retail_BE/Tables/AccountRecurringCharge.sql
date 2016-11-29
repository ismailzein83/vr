CREATE TABLE [Retail_BE].[AccountRecurringCharge] (
    [AccountID]                BIGINT           NOT NULL,
    [ChargingDefinitionItemId] UNIQUEIDENTIFIER NOT NULL,
    [NextChargingTime]         DATETIME         NOT NULL,
    [LastChargingTime]         DATETIME         NULL,
    [CreatedTime]              DATETIME         CONSTRAINT [DF_AccountRecurringCharge_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_AccountRecurringCharge] PRIMARY KEY CLUSTERED ([AccountID] ASC, [ChargingDefinitionItemId] ASC)
);

