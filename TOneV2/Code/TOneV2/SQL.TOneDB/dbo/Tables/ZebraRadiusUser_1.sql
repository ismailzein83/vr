CREATE TABLE [dbo].[ZebraRadiusUser] (
    [CarrierAccountID]     VARCHAR (5)     NOT NULL,
    [RequestID]            INT             NOT NULL,
    [ActivationStatus]     TINYINT         NOT NULL,
    [ActivationStatusType] TINYINT         NULL,
    [Balance]              DECIMAL (18, 3) NULL,
    [CreationDate]         DATETIME        NULL,
    [LastUpdated]          DATETIME        NULL,
    [CustomerCreditLimit]  INT             NULL
);

