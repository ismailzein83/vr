CREATE TABLE [Mediation_WHS].[JazzHuaweiBadCDR] (
    [Id]                   BIGINT           NOT NULL,
    [Direction]            VARCHAR (5)      NULL,
    [SwitchType]           VARCHAR (5)      NULL,
    [InTrunk]              VARCHAR (25)     NULL,
    [OutTrunk]             VARCHAR (25)     NULL,
    [Anum]                 VARCHAR (25)     NULL,
    [Bnum]                 VARCHAR (25)     NULL,
    [StartDate]            DATE             NULL,
    [StartTime]            TIME (3)         NULL,
    [Duration]             DECIMAL (20, 4)  NULL,
    [UserData]             VARCHAR (25)     NULL,
    [ICTRecordType]        VARCHAR (25)     NULL,
    [causeForTermination]  VARCHAR (5)      NULL,
    [reasonForTermination] VARCHAR (5)      NULL,
    [callReferenceNumber]  VARCHAR (25)     NULL,
    [drcCallId]            VARCHAR (25)     NULL,
    [sequenceNumber]       BIGINT           NULL,
    [callTransactionType]  VARCHAR (25)     NULL,
    [ThirdNumber]          VARCHAR (25)     NULL,
    [Rfu1]                 VARCHAR (25)     NULL,
    [Rfu2]                 VARCHAR (25)     NULL,
    [Rfu3]                 VARCHAR (25)     NULL,
    [Rfu4]                 VARCHAR (25)     NULL,
    [Rfu5]                 VARCHAR (25)     NULL,
    [FileName]             VARCHAR (255)    NULL,
    [IgnoreTransitRule]    BIT              NULL,
    [DatasourceId]         UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_JazzHuaweiBadCDR] PRIMARY KEY CLUSTERED ([Id] ASC)
);











