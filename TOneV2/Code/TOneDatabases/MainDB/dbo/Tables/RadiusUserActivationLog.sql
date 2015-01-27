CREATE TABLE [dbo].[RadiusUserActivationLog] (
    [ID]                   INT             IDENTITY (1, 1) NOT NULL,
    [CarrierAccountID]     VARCHAR (5)     NOT NULL,
    [RequestID]            INT             NOT NULL,
    [ActivationStatus]     TINYINT         NOT NULL,
    [ActivationStatusType] TINYINT         NULL,
    [Balance]              DECIMAL (18, 3) NULL,
    [CreationDate]         DATETIME        NULL,
    [LastUpdated]          DATETIME        NULL,
    [CustomerCreditLimit]  INT             NULL,
    [IsApplied]            BIT             NULL,
    [OrderType]            TINYINT         NULL,
    CONSTRAINT [PK_RadiusUserActivationLog] PRIMARY KEY CLUSTERED ([ID] ASC)
);

