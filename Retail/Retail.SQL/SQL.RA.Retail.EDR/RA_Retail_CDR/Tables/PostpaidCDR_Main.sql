﻿CREATE TABLE [RA_Retail_CDR].[PostpaidCDR_Main] (
    [ID]                       BIGINT           NOT NULL,
    [OperatorID]               BIGINT           NULL,
    [DataSourceID]             UNIQUEIDENTIFIER NULL,
    [ProbeID]                  BIGINT           NULL,
    [AttemptDateTime]          DATETIME         NULL,
    [ConnectDateTime]          DATETIME         NULL,
    [DisconnectDateTime]       DATETIME         NULL,
    [DurationInSeconds]        DECIMAL (20, 4)  NULL,
    [SaleDurationInSeconds]    DECIMAL (20, 4)  NULL,
    [CDPN]                     NVARCHAR (255)   NULL,
    [CGPN]                     NVARCHAR (255)   NULL,
    [TrafficDirection]         INT              NULL,
    [DestinationZoneID]        BIGINT           NULL,
    [OriginationZoneID]        BIGINT           NULL,
    [Revenue]                  DECIMAL (22, 8)  NULL,
    [TaxRuleID]                INT              NULL,
    [Income]                   DECIMAL (22, 8)  NULL,
    [AlertDateTime]            DATETIME         NULL,
    [PDDInSeconds]             DECIMAL (20, 4)  NULL,
    [SubscriberID]             BIGINT           NULL,
    [ChargingPolicyID]         INT              NULL,
    [PackageID]                BIGINT           NULL,
    [SaleRate]                 DECIMAL (22, 8)  NULL,
    [SaleAmount]               DECIMAL (22, 8)  NULL,
    [SaleRateTypeID]           INT              NULL,
    [SaleCurrencyID]           INT              NULL,
    [SaleRateValueRuleID]      INT              NULL,
    [SaleRateTypeRuleID]       INT              NULL,
    [SaleTariffRuleID]         INT              NULL,
    [SaleExtraChargeRuleID]    INT              NULL,
    [ChargedDurationInSeconds] DECIMAL (20, 4)  NULL,
    [ServiceTypeID]            UNIQUEIDENTIFIER NULL,
    [CDRType]                  INT              NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

