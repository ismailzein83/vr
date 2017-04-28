﻿CREATE TABLE [Retail_EDR].[GPRS] (
    [ID]              BIGINT           IDENTITY (1, 1) NOT NULL,
    [StartDate]       DATETIME         NULL,
    [TypeGprs]        VARCHAR (32)     NULL,
    [Calling]         VARCHAR (32)     NULL,
    [Zone]            VARCHAR (256)    NULL,
    [Bill]            INT              NULL,
    [Credit]          DECIMAL (20, 10) NULL,
    [TrafficType]     VARCHAR (32)     NULL,
    [Unit]            VARCHAR (24)     NULL,
    [Balance]         DECIMAL (20, 10) NULL,
    [Bag]             VARCHAR (256)    NULL,
    [Amount]          DECIMAL (20, 10) NULL,
    [TypeConsumed]    VARCHAR (32)     NULL,
    [PricePlan]       VARCHAR (64)     NULL,
    [Promotion]       VARCHAR (64)     NULL,
    [AccessPointName] VARCHAR (64)     NULL,
    [ParentIdCDR]     BIGINT           NULL,
    [IdCDR]           BIGINT           NULL,
    [IdCdrGprs]       BIGINT           NULL,
    [FileName]        VARCHAR (64)     NULL,
    [FileDate]        DATETIME         NULL,
    [CreationDate]    DATETIME         NULL,
    [AirtimeRate]     DECIMAL (20, 10) NULL,
    [AirtimeAmount]   DECIMAL (20, 10) NULL,
    [AgentCommission] DECIMAL (20, 10) NULL,
    [Agent]           BIGINT           NULL,
    [Account]         BIGINT           NULL,
    [Profit]          DECIMAL (20, 10) NULL
);







