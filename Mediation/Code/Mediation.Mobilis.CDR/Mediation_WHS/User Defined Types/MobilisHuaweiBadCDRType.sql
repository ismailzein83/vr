﻿CREATE TYPE [Mediation_WHS].[MobilisHuaweiBadCDRType] AS TABLE (
    [Id]                           BIGINT           NULL,
    [SerialNumber]                 BIGINT           NULL,
    [TicketType]                   INT              NULL,
    [CheckSum]                     INT              NULL,
    [PartialRecordIndicator]       INT              NULL,
    [ClockChangedFlag]             BIT              NULL,
    [FreeFlag]                     INT              NULL,
    [Validity]                     INT              NULL,
    [CallAttemptFlag]              INT              NULL,
    [ComplaintFlag]                INT              NULL,
    [CentralizedChargingFlag]      BIT              NULL,
    [PPSFlag]                      BIT              NULL,
    [ChargingMethod]               INT              NULL,
    [NPCallFlag]                   BIT              NULL,
    [Payer]                        INT              NULL,
    [ConversationBeginTime]        DATETIME         NULL,
    [ConversationEndTime]          DATETIME         NULL,
    [ConversationDuration]         BIGINT           NULL,
    [CallerSeizureDuration]        BIGINT           NULL,
    [CalledSeizureDuration]        BIGINT           NULL,
    [IncompleteCallWatch]          INT              NULL,
    [CallerISDNAccess]             BIT              NULL,
    [CalledISDNAccess]             BIT              NULL,
    [ISUPIndication]               BIT              NULL,
    [ChargingNumberAddressNature]  INT              NULL,
    [CallerNumberAddressNature]    INT              NULL,
    [ConnectedNumberAddressNature] INT              NULL,
    [CalledNumberAddressNature]    INT              NULL,
    [ChargingNumberDNSet]          INT              NULL,
    [ChargingNumber]               VARCHAR (50)     NULL,
    [CallerNumberDNSet]            INT              NULL,
    [CallerNumber]                 VARCHAR (50)     NULL,
    [ConnectedNumberDNSet]         INT              NULL,
    [ConnectedNumber]              VARCHAR (50)     NULL,
    [CalledNumberDNSet]            INT              NULL,
    [CalledNumber]                 VARCHAR (50)     NULL,
    [DialedNumber]                 VARCHAR (50)     NULL,
    [CentrexGroupNumber]           INT              NULL,
    [CallerCentrexShortNumber]     BIGINT           NULL,
    [CalledCentrexShortNumber]     BIGINT           NULL,
    [CallerModuleNumber]           INT              NULL,
    [CalledModuleNumber]           INT              NULL,
    [IncomingTrunkGroupNumber]     INT              NULL,
    [OutgoingTrunkGroupNumber]     INT              NULL,
    [IncomingSubrouteNumber]       INT              NULL,
    [OutgoingSubrouteNumber]       INT              NULL,
    [CallerDeviceType]             INT              NULL,
    [CalledDeviceType]             INT              NULL,
    [CallerPortNumber]             INT              NULL,
    [CalledPortNumber]             INT              NULL,
    [CallerCategory]               INT              NULL,
    [CalledCategory]               INT              NULL,
    [CallType]                     INT              NULL,
    [ServiceType]                  INT              NULL,
    [SupplementaryServiceType]     INT              NULL,
    [ChargingCase]                 INT              NULL,
    [Tariff]                       INT              NULL,
    [ChargingPulse]                BIGINT           NULL,
    [Fee]                          BIGINT           NULL,
    [Balance]                      BIGINT           NULL,
    [BearerService]                INT              NULL,
    [Teleservice]                  INT              NULL,
    [ReleaseParty]                 INT              NULL,
    [ReleaseIndex]                 INT              NULL,
    [ReleaseCauseValue]            INT              NULL,
    [UUS1Count]                    INT              NULL,
    [UUS2Count]                    INT              NULL,
    [UUS3Count]                    INT              NULL,
    [OPC]                          BIGINT           NULL,
    [DPC]                          BIGINT           NULL,
    [B_Num]                        INT              NULL,
    [FileName]                     VARCHAR (255)    NULL,
    [DataSourceId]                 UNIQUEIDENTIFIER NULL);
