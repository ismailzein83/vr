CREATE TABLE [Retail_EDR].[Message] (
    [ID]                  BIGINT           IDENTITY (1, 1) NOT NULL,
    [StartDate]           DATETIME         NULL,
    [IdCDR]               BIGINT           NULL,
    [TrafficType]         VARCHAR (32)     NULL,
    [DirectionTraffic]    VARCHAR (32)     NULL,
    [Calling]             VARCHAR (32)     NULL,
    [Called]              VARCHAR (32)     NULL,
    [TypeNet]             VARCHAR (32)     NULL,
    [SourceOperator]      VARCHAR (32)     NULL,
    [DestinationOperator] VARCHAR (32)     NULL,
    [SourceArea]          VARCHAR (256)    NULL,
    [DestinationArea]     VARCHAR (256)    NULL,
    [Bill]                INT              NULL,
    [Credit]              DECIMAL (20, 10) NULL,
    [Unit]                VARCHAR (256)    NULL,
    [Balance]             DECIMAL (20, 10) NULL,
    [Bag]                 VARCHAR (256)    NULL,
    [Amount]              DECIMAL (20, 10) NULL,
    [TypeConsumed]        VARCHAR (32)     NULL,
    [PricePlan]           VARCHAR (64)     NULL,
    [Promotion]           VARCHAR (64)     NULL,
    [ParentIdCDR]         BIGINT           NULL,
    [FileName]            VARCHAR (64)     NULL,
    [FileDate]            DATETIME         NULL,
    [CreationDate]        DATETIME         NULL,
    [TypeMessage]         VARCHAR (32)     NULL,
    [Zone]                BIGINT           NULL,
    [AgentID]             BIGINT           NULL,
    [AgentCommission]     DECIMAL (20, 10) NULL,
    [AccountID]           BIGINT           NULL,
    [OriginatingZoneID]   BIGINT           NULL,
    [TerminatingZoneID]   BIGINT           NULL,
    [AirtimeRate]         DECIMAL (20, 10) NULL,
    [AirtimeAmount]       DECIMAL (20, 10) NULL,
    [TerminationRate]     DECIMAL (20, 10) NULL,
    [TerminationAmount]   DECIMAL (20, 10) NULL
);







