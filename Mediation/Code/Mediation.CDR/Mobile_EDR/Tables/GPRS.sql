﻿CREATE TABLE [Mobile_EDR].[GPRS] (
    [Id]                         BIGINT        IDENTITY (1, 1) NOT NULL,
    [RecordType]                 INT           NULL,
    [NetworkInitiation]          BIT           NULL,
    [ServedIMSI]                 VARCHAR (100) NULL,
    [ServedIMEI]                 VARCHAR (100) NULL,
    [SGSN_Address]               VARCHAR (100) NULL,
    [MSNetworkCapability]        VARCHAR (100) NULL,
    [RoutingArea]                INT           NULL,
    [LocationAreaCode]           INT           NULL,
    [CellIdentifier]             INT           NULL,
    [ChargingID]                 INT           NULL,
    [GGSN_Address]               VARCHAR (100) NULL,
    [AccessPointNameNI]          VARCHAR (100) NULL,
    [PDPType]                    VARCHAR (100) NULL,
    [ServedPDPAddress]           VARCHAR (100) NULL,
    [RecordOpeningTime]          DATETIME      NULL,
    [Duration]                   INT           NULL,
    [SGSN_Change]                BIT           NULL,
    [CauseForRecClosing]         INT           NULL,
    [Diagnostics]                INT           NULL,
    [RecordSequenceNumber]       INT           NULL,
    [NodeID]                     VARCHAR (100) NULL,
    [RecordExtensions]           VARCHAR (100) NULL,
    [LocalSequenceNumber]        INT           NULL,
    [APNSelectionMode]           INT           NULL,
    [AccessPointNameOI]          VARCHAR (100) NULL,
    [ServedMSISDN]               VARCHAR (100) NULL,
    [ChargingCharacteristics]    INT           NULL,
    [SystemType]                 INT           NULL,
    [SCFAddress]                 VARCHAR (100) NULL,
    [ServiceKey]                 VARCHAR (100) NULL,
    [DefaultTransactionHandling] INT           NULL,
    [CamelAccessPointNameNI]     VARCHAR (100) NULL,
    [CamelAccessPointNameOI]     VARCHAR (100) NULL,
    [NumberOfDPEncountered]      INT           NULL,
    [LevelOfCamelService]        VARCHAR (100) NULL,
    [FreeFormatData]             VARCHAR (100) NULL,
    [FFDAppendIndicator]         BIT           NULL,
    [RNC_UnsentDownlinkVolume]   INT           NULL,
    [ChSelectionMode]            INT           NULL,
    [DynamicAddressFlag]         BIT           NULL,
    [QosRequested]               VARCHAR (100) NULL,
    [QosNegotiated]              VARCHAR (100) NULL,
    [DataVolumeGPRSUplink]       INT           NULL,
    [DataVolumeGPRSDownlink]     INT           NULL,
    [ChangeCondition]            INT           NULL,
    [ChangeTime]                 DATETIME      NULL,
    CONSTRAINT [PK_GPRS] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_GPRS_RecordOpeningTime]
    ON [Mobile_EDR].[GPRS]([RecordOpeningTime] ASC);

