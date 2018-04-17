CREATE TABLE [Mediation_WHS].[JazzHuaweiBadCDR] (
    [Id]                   BIGINT          NOT NULL,
    [EventDirection]       VARCHAR (5)     NULL,
    [IncomingSwtich]       VARCHAR (5)     NULL,
    [OutgoingSwitch]       VARCHAR (5)     NULL,
    [IncTrunk]             VARCHAR (25)    NULL,
    [OutTrunk]             VARCHAR (25)    NULL,
    [IncProduct]           VARCHAR (25)    NULL,
    [OutProduct]           VARCHAR (25)    NULL,
    [OrigANumber]          VARCHAR (25)    NULL,
    [OrigBNumber]          VARCHAR (25)    NULL,
    [ANumber]              VARCHAR (25)    NULL,
    [BNumber]              VARCHAR (25)    NULL,
    [StartDate]            DATE            NULL,
    [StartTime]            TIME (3)        NULL,
    [DurationInSeconds]    DECIMAL (20, 4) NULL,
    [NetStartDate]         DATE            NULL,
    [NetStartTime]         TIME (3)        NULL,
    [NetDurationInSeconds] DECIMAL (20, 4) NULL,
    [DataVolume]           VARCHAR (5)     NULL,
    [DataUnit]             VARCHAR (5)     NULL,
    [UserType]             VARCHAR (5)     NULL,
    [IMSINumber]           VARCHAR (25)    NULL,
    [ServiceClass]         VARCHAR (5)     NULL,
    [TELEServNumber]       INT             NULL,
    [Cell_Id]              VARCHAR (25)    NULL,
    [RecordType]           VARCHAR (10)    NULL,
    [FileName]             VARCHAR (255)   NULL,
    CONSTRAINT [PK_JazzHuaweiBadCDR] PRIMARY KEY CLUSTERED ([Id] ASC)
);



