﻿CREATE TABLE [FraudAnalysis].[NormalCDR] (
    [MSISDN]              VARCHAR (30)    NOT NULL,
    [IMSI]                VARCHAR (20)    NULL,
    [ConnectDateTime]     DATETIME        NOT NULL,
    [Destination]         VARCHAR (40)    NULL,
    [DurationInSeconds]   NUMERIC (13, 4) NOT NULL,
    [DisconnectDateTime]  DATETIME        NULL,
    [CallClassID]         INT             NULL,
    [IsOnNet]             BIT             NOT NULL,
    [CallTypeID]          INT             NOT NULL,
    [SubscriberTypeID]    INT             NULL,
    [IMEI]                VARCHAR (20)    NULL,
    [BTS]                 VARCHAR (50)    NULL,
    [Cell]                VARCHAR (50)    NULL,
    [SwitchID]            INT             NULL,
    [UpVolume]            DECIMAL (18, 2) NULL,
    [DownVolume]          DECIMAL (18, 2) NULL,
    [CellLatitude]        DECIMAL (18, 8) NULL,
    [CellLongitude]       DECIMAL (18, 8) NULL,
    [ServiceTypeID]       INT             NULL,
    [ServiceVASName]      VARCHAR (50)    NULL,
    [InTrunkID]           INT             NULL,
    [OutTrunkID]          INT             NULL,
    [ReleaseCode]         VARCHAR (50)    NULL,
    [MSISDNAreaCode]      VARCHAR (10)    NULL,
    [DestinationAreaCode] VARCHAR (10)    NULL
);
















GO
CREATE CLUSTERED INDEX [IX_NormalCDR_MSISDN]
    ON [FraudAnalysis].[NormalCDR]([MSISDN] ASC, [ConnectDateTime] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_NormalCDR_ConnectDateTime]
    ON [FraudAnalysis].[NormalCDR]([ConnectDateTime] ASC);

