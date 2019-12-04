﻿CREATE TABLE [RetailBilling].[ContractServiceAction] (
    [ID]                            BIGINT           IDENTITY (1, 1) NOT NULL,
    [ContractServiceID]             BIGINT           NULL,
    [ActionTypeID]                  UNIQUEIDENTIFIER NULL,
    [Charge]                        DECIMAL (26, 8)  NULL,
    [EvaluatedCharge]               DECIMAL (26, 8)  NULL,
    [ChargeTime]                    DATETIME         NULL,
    [PaidCash]                      BIT              NULL,
    [ContractServiceHistoryID]      BIGINT           NULL,
    [OldServiceOptionID]            UNIQUEIDENTIFIER NULL,
    [NewServiceOptionID]            UNIQUEIDENTIFIER NULL,
    [OldServiceOptionActivationFee] DECIMAL (26, 8)  NULL,
    [NewServiceOptionActivationFee] DECIMAL (26, 8)  NULL,
    [OldSpeedInMbps]                DECIMAL (20, 4)  NULL,
    [NewSpeedInMbps]                DECIMAL (20, 4)  NULL,
    [CreatedTime]                   DATETIME         NULL,
    [CreatedBy]                     INT              NULL,
    [LastModifiedTime]              DATETIME         NULL,
    [LastModifiedBy]                INT              NULL,
    CONSTRAINT [PK__Contract__3214EC270B5CAFEA] PRIMARY KEY CLUSTERED ([ID] ASC)
);



