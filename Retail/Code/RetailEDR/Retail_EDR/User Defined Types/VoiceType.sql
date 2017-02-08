CREATE TYPE [Retail_EDR].[VoiceType] AS TABLE (
    [ID]                  BIGINT           NULL,
    [IdCDR]               BIGINT           NULL,
    [StartDate]           DATETIME         NULL,
    [ParentIdCDR]         BIGINT           NULL,
    [TrafficType]         VARCHAR (32)     NULL,
    [DirectionTraffic]    VARCHAR (32)     NULL,
    [Calling]             VARCHAR (32)     NULL,
    [Called]              VARCHAR (32)     NULL,
    [RedirectingNumber]   VARCHAR (32)     NULL,
    [TypeNet]             VARCHAR (32)     NULL,
    [SourceOperator]      VARCHAR (32)     NULL,
    [DestinationOperator] VARCHAR (32)     NULL,
    [SourceArea]          VARCHAR (256)    NULL,
    [DestinationArea]     VARCHAR (256)    NULL,
    [Duration]            DECIMAL (20, 10) NULL,
    [DurationUnit]        VARCHAR (24)     NULL,
    [Amount]              DECIMAL (20, 10) NULL,
    [TypeConsumed]        VARCHAR (32)     NULL,
    [Bag]                 VARCHAR (256)    NULL,
    [PricePlan]           VARCHAR (64)     NULL,
    [Promotion]           VARCHAR (64)     NULL,
    [FileName]            VARCHAR (64)     NULL,
    [FileDate]            DATETIME         NULL,
    [CreationDate]        DATETIME         NULL,
    [Balance]             DECIMAL (20, 10) NULL,
    [MTCost]              VARCHAR (256)    NULL,
    [TermDesc]            VARCHAR (256)    NULL,
    [TypeCalled]          VARCHAR (32)     NULL,
    [Agent]               BIGINT           NULL,
    [AgentCommission]     DECIMAL (20, 10) NULL,
    [AccountID]           BIGINT           NULL,
    [TerminatingZoneID]   BIGINT           NULL,
    [OriginatingZoneID]   BIGINT           NULL,
    [AirtimeRate]         DECIMAL (20, 10) NULL,
    [AirtimeAmount]       DECIMAL (20, 10) NULL,
    [TerminationRate]     DECIMAL (20, 10) NULL,
    [TerminationAmount]   DECIMAL (20, 10) NULL);







