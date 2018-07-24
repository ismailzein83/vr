﻿CREATE TABLE [Mediation_WHS].[OgeroHuaweiEPCCDR] (
    [Id]                             BIGINT        NULL,
    [RecordType]                     INT           NULL,
    [ServedMSI]                      VARCHAR (50)  NULL,
    [PGWAddress]                     VARCHAR (50)  NULL,
    [ChargingID]                     BIGINT        NULL,
    [ServingNodeAddress]             VARCHAR (50)  NULL,
    [AccessPointNameNI]              VARCHAR (255) NULL,
    [ServedPDPPDNAddress]            VARCHAR (50)  NULL,
    [DynamicAddressFlag]             VARCHAR (50)  NULL,
    [DataVolumeGPRSUplink]           BIGINT        NULL,
    [DataVolumeGPRSDownlink]         BIGINT        NULL,
    [ChangeCondition]                INT           NULL,
    [ChangeTime]                     DATETIME      NULL,
    [FailureHandlingContinue]        VARCHAR (50)  NULL,
    [CPCIoTEPSOptimisationIndicator] VARCHAR (50)  NULL,
    [RecordOpeningTime]              DATETIME      NULL,
    [Duration]                       INT           NULL,
    [CauseForRecClosing]             INT           NULL,
    [RecordSequenceNumber]           INT           NULL,
    [NodeID]                         VARCHAR (50)  NULL,
    [APNSelectionMode]               INT           NULL,
    [ServedMSISDN]                   VARCHAR (50)  NULL,
    [ChargingCharacteristics]        VARCHAR (50)  NULL,
    [ChSelectionMode]                INT           NULL,
    [ServingNodePLMNIdentifier]      VARCHAR (50)  NULL,
    [ServedIMEISV]                   VARCHAR (50)  NULL,
    [RATType]                        INT           NULL,
    [RatingGroup]                    INT           NULL,
    [LocalSequenceNumber]            BIGINT        NULL,
    [TimeOfFirstUsage]               DATETIME      NULL,
    [TimeOfLastUsage]                DATETIME      NULL,
    [TimeUsage]                      INT           NULL,
    [ServiceConditionChange]         VARCHAR (255) NULL,
    [DataVolumeFBCUplink]            BIGINT        NULL,
    [DataVolumeFBCDownlink]          BIGINT        NULL,
    [TimeOfReport]                   DATETIME      NULL,
    [ServingNodeType]                INT           NULL
);

