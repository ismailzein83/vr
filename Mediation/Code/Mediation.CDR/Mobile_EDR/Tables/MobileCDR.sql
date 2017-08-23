CREATE TABLE [Mobile_EDR].[MobileCDR] (
    [Id]                            BIGINT           IDENTITY (1, 1) NOT NULL,
    [RecordType]                    INT              NULL,
    [CallingNumber]                 VARCHAR (100)    NULL,
    [CalledNumber]                  VARCHAR (100)    NULL,
    [CallDuration]                  INT              NULL,
    [SetupTime]                     DATETIME         NULL,
    [CalledIMSI]                    VARCHAR (100)    NULL,
    [CalledIMEI]                    VARCHAR (100)    NULL,
    [ConnectDateTime]               DATETIME         NULL,
    [DisconnectDateTime]            DATETIME         NULL,
    [SwitchID]                      INT              NULL,
    [CallingIMSI]                   VARCHAR (50)     NULL,
    [CallingIMEI]                   VARCHAR (50)     NULL,
    [ConnectTimestamp]              BIGINT           NULL,
    [DisconnectTimestamp]           BIGINT           NULL,
    [UniqueIdentifier]              UNIQUEIDENTIFIER NULL,
    [FileName]                      NVARCHAR (200)   NULL,
    [Called_First_CI]               VARCHAR (100)    NULL,
    [Called_Last_CI]                VARCHAR (100)    NULL,
    [Calling_First_CI]              VARCHAR (100)    NULL,
    [Calling_Last_CI]               VARCHAR (100)    NULL,
    [RecordTypeName]                VARCHAR (100)    NULL,
    [IntermediateChargingIndicator] INT              NULL,
    [CallReference]                 BIGINT           NULL,
    [GlobalCallReference]           VARCHAR (200)    NULL,
    [IntermediateRecordNumber]      INT              NULL,
    [OutgoingRoute]                 VARCHAR (50)     NULL,
    [IncomingRoute]                 VARCHAR (50)     NULL,
    CONSTRAINT [PK_MobileCDR_1] PRIMARY KEY CLUSTERED ([Id] ASC)
);




























GO


