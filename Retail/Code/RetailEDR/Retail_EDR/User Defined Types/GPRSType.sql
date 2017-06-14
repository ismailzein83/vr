CREATE TYPE [Retail_EDR].[GPRSType] AS TABLE (
    [ID]              BIGINT           NULL,
    [StartDate]       DATETIME         NULL,
    [TypeGprs]        VARCHAR (256)    NULL,
    [Calling]         VARCHAR (256)    NULL,
    [Zone]            VARCHAR (256)    NULL,
    [Bill]            INT              NULL,
    [Credit]          DECIMAL (20, 10) NULL,
    [TrafficType]     VARCHAR (32)     NULL,
    [Unit]            VARCHAR (24)     NULL,
    [Balance]         DECIMAL (25, 10) NULL,
    [Bag]             VARCHAR (256)    NULL,
    [Amount]          DECIMAL (20, 10) NULL,
    [TypeConsumed]    VARCHAR (256)    NULL,
    [PricePlan]       VARCHAR (256)    NULL,
    [Promotion]       VARCHAR (256)    NULL,
    [AccessPointName] VARCHAR (256)    NULL,
    [ParentIdCDR]     BIGINT           NULL,
    [IdCDR]           BIGINT           NULL,
    [IdCdrGprs]       BIGINT           NULL,
    [FileName]        VARCHAR (256)    NULL,
    [FileDate]        DATETIME         NULL,
    [CreationDate]    DATETIME         NULL,
    [AirtimeRate]     DECIMAL (20, 10) NULL,
    [AirtimeAmount]   DECIMAL (20, 10) NULL,
    [Agent]           BIGINT           NULL,
    [AgentCommission] DECIMAL (20, 10) NULL,
    [Account]         BIGINT           NULL,
    [Profit]          DECIMAL (20, 10) NULL);









