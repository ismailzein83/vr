﻿CREATE TABLE [Mobile_EDR].[MobileCDR_Full] (
    [Id]                                BIGINT        IDENTITY (1, 1) NOT NULL,
    [RecordType]                        INT           NULL,
    [ServedIMSI]                        VARCHAR (100) NULL,
    [ServedIMEI]                        VARCHAR (100) NULL,
    [ServedMSISDN]                      VARCHAR (100) NULL,
    [CallingNumber]                     VARCHAR (100) NULL,
    [CalledNumber]                      VARCHAR (100) NULL,
    [TranslatedNumber]                  VARCHAR (100) NULL,
    [ConnectedNumber]                   VARCHAR (100) NULL,
    [RoamingNumber]                     VARCHAR (100) NULL,
    [RecordingEntity]                   VARCHAR (100) NULL,
    [MSCIncomingROUTE]                  VARCHAR (100) NULL,
    [MSCOutgoingROUTE]                  VARCHAR (100) NULL,
    [BasicService]                      VARCHAR (100) NULL,
    [TransparencyIndicator]             INT           NULL,
    [MSClassmark]                       VARCHAR (100) NULL,
    [SeizureTime]                       DATETIME      NULL,
    [AnswerTime]                        DATETIME      NULL,
    [ReleaseTime]                       DATETIME      NULL,
    [CallDuration]                      INT           NULL,
    [RadioChanRequested]                INT           NULL,
    [RadioChanUsed]                     INT           NULL,
    [CauseForTerm]                      INT           NULL,
    [Diagnostics]                       VARCHAR (100) NULL,
    [CallReference]                     VARCHAR (100) NULL,
    [SequenceNumber]                    INT           NULL,
    [GSM_SCFAddress]                    VARCHAR (100) NULL,
    [ServiceKey]                        INT           NULL,
    [NetworkCallReference]              VARCHAR (100) NULL,
    [MSCAddress]                        VARCHAR (100) NULL,
    [CamelInitCFIndicator]              BIT           NULL,
    [DefaultCallHandling]               INT           NULL,
    [FNUR]                              INT           NULL,
    [AIURRequested]                     INT           NULL,
    [SpeechVersionSupported]            VARCHAR (100) NULL,
    [NumberOfDPEncountered]             INT           NULL,
    [SpeechVersionUsed]                 VARCHAR (100) NULL,
    [LevelOfCAMELService]               VARCHAR (100) NULL,
    [FreeFormatData]                    VARCHAR (100) NULL,
    [CamelCallLegInformation]           VARCHAR (100) NULL,
    [FreeFormatDataAppend]              BIT           NULL,
    [DefaultCallHandling_2]             INT           NULL,
    [GSM_SCFAddress_2]                  VARCHAR (100) NULL,
    [ServiceKey_2]                      INT           NULL,
    [FreeFormatData_2]                  VARCHAR (100) NULL,
    [FreeFormatDataAppend_2]            BIT           NULL,
    [SystemType]                        INT           NULL,
    [RateIndication]                    VARCHAR (100) NULL,
    [PartialRecordType]                 INT           NULL,
    [GuaranteedBitRate]                 INT           NULL,
    [MaximumBitRate]                    INT           NULL,
    [USSDCallBackFlag]                  VARCHAR (100) NULL,
    [ModemType]                         INT           NULL,
    [Classmark3]                        VARCHAR (100) NULL,
    [ChargedParty]                      INT           NULL,
    [OriginalCalledNumber]              VARCHAR (100) NULL,
    [ChargeAreaCode]                    VARCHAR (100) NULL,
    [CalledChargeAreaCode]              VARCHAR (100) NULL,
    [MSCCircuit]                        INT           NULL,
    [OriginatingRNCorBSCId]             VARCHAR (100) NULL,
    [OriginatingMSCId]                  VARCHAR (100) NULL,
    [CallEmlppPriority]                 VARCHAR (100) NULL,
    [DefaultEmlppPriority]              VARCHAR (100) NULL,
    [EASubscriberInfo]                  VARCHAR (100) NULL,
    [SelectedCIC]                       VARCHAR (100) NULL,
    [OptimalRoutingFlag]                VARCHAR (100) NULL,
    [OptimalRoutingLateForwardFlag]     VARCHAR (100) NULL,
    [OptimalRoutingEarlyForwardFlag]    VARCHAR (100) NULL,
    [CallerPortedFlag]                  INT           NULL,
    [CalledIMSI]                        VARCHAR (100) NULL,
    [GlobalAreaID]                      VARCHAR (100) NULL,
    [ChangeOfGlobalAreaID]              VARCHAR (100) NULL,
    [SubscriberCategory]                VARCHAR (100) NULL,
    [FirstMCC_MNC]                      VARCHAR (100) NULL,
    [IntermediateMCC_MNC]               VARCHAR (100) NULL,
    [LastMCC_MNC]                       VARCHAR (100) NULL,
    [CUGOutgoingAccessIndicator]        INT           NULL,
    [CUGInterlockCode]                  VARCHAR (100) NULL,
    [CUGAccessUsed]                     INT           NULL,
    [CUGIndex]                          VARCHAR (100) NULL,
    [InteractionWithIP]                 VARCHAR (100) NULL,
    [HotBillingTag]                     INT           NULL,
    [SetupTime]                         DATETIME      NULL,
    [AlertingTime]                      DATETIME      NULL,
    [VoiceIndicator]                    INT           NULL,
    [BCategory]                         INT           NULL,
    [CallType]                          INT           NULL,
    [ResourceChargeIPNumber]            VARCHAR (100) NULL,
    [CamelDestinationNumber]            VARCHAR (100) NULL,
    [GroupCallType]                     INT           NULL,
    [GroupCallReference]                VARCHAR (100) NULL,
    [UUS1Type]                          INT           NULL,
    [ECategory]                         INT           NULL,
    [TariffCode]                        INT           NULL,
    [DisconnectParty]                   INT           NULL,
    [ChargePulseNum]                    INT           NULL,
    [CSReference]                       VARCHAR (100) NULL,
    [CSAReference]                      INT           NULL,
    [CamelPhase]                        INT           NULL,
    [NetworkOperatorId]                 VARCHAR (100) NULL,
    [TypeOfSubscribers]                 INT           NULL,
    [AudioDataType]                     INT           NULL,
    [UserType]                          INT           NULL,
    [RecordNumber]                      INT           NULL,
    [OSSSServicesUsed]                  VARCHAR (100) NULL,
    [PartyRelCause]                     VARCHAR (100) NULL,
    [ChargeLevel]                       INT           NULL,
    [LocationNum]                       VARCHAR (100) NULL,
    [ZoneCode]                          VARCHAR (100) NULL,
    [LocationNumberNai]                 INT           NULL,
    [DTMF_Indicator]                    BIT           NULL,
    [BChannelNumber]                    INT           NULL,
    [NCNPFlag]                          INT           NULL,
    [MCTType]                           INT           NULL,
    [CARP]                              INT           NULL,
    [AccountCode]                       VARCHAR (100) NULL,
    [ChannelMode]                       INT           NULL,
    [Channel]                           INT           NULL,
    [ICIDValue]                         VARCHAR (100) NULL,
    [SpecialBillPrefix]                 VARCHAR (100) NULL,
    [Termioi]                           VARCHAR (100) NULL,
    [Origioi]                           VARCHAR (100) NULL,
    [CalledPortedFlag]                  INT           NULL,
    [LocationRoutingNumber]             VARCHAR (100) NULL,
    [RoutingCategory]                   VARCHAR (100) NULL,
    [IntermediateChargingInd]           INT           NULL,
    [CalledIMEI]                        VARCHAR (100) NULL,
    [MSCOutgoingROUTENumber]            INT           NULL,
    [MSCIncomingROUTENumber]            INT           NULL,
    [RODefaultCallHandling]             INT           NULL,
    [ROLinkFailureTime]                 DATETIME      NULL,
    [LastSuccCCRTime]                   DATETIME      NULL,
    [DRCCallId]                         VARCHAR (100) NULL,
    [DRCCallRN]                         VARCHAR (100) NULL,
    [NPDipIndicator]                    VARCHAR (100) NULL,
    [ANSIRoutingNumber]                 VARCHAR (100) NULL,
    [lRNSource]                         INT           NULL,
    [WPSCallFlag]                       INT           NULL,
    [RedirectingCounter]                INT           NULL,
    [VOBBUserFlag]                      VARCHAR (100) NULL,
    [ChargePulses]                      INT           NULL,
    [VasType]                           INT           NULL,
    [CallRedirectionFlag]               INT           NULL,
    [OfficeName]                        VARCHAR (100) NULL,
    [SCPConnection]                     INT           NULL,
    [ChargeClass]                       INT           NULL,
    [npaNxx]                            VARCHAR (100) NULL,
    [GlobalCallReference]               VARCHAR (100) NULL,
    [CalledLastCI]                      VARCHAR (100) NULL,
    [CallDropInd]                       VARCHAR (100) NULL,
    [CauseLocation]                     INT           NULL,
    [IntermediateRate]                  INT           NULL,
    [RetrievalOfHeldCall]               INT           NULL,
    [PresentationAndScreeningIndicator] VARCHAR (100) NULL,
    [CallingNIR]                        VARCHAR (100) NULL,
    [InSupplementaryServiceValue]       VARCHAR (100) NULL,
    [CalledNumCategory]                 VARCHAR (100) NULL,
    [GroupID]                           INT           NULL,
    [VPNCallProperty]                   INT           NULL,
    [SubgroupID]                        VARCHAR (100) NULL,
    [AccessNetworkInformation]          VARCHAR (100) NULL,
    [RingingDuration]                   INT           NULL,
    [CallProperty]                      INT           NULL,
    [SDPMediaIdentifier]                INT           NULL,
    [ServedPartyIPAddress]              VARCHAR (100) NULL,
    [NumberPortabilityStatus]           INT           NULL,
    [AnchorFlag]                        VARCHAR (100) NULL,
    [ImsServiceCode]                    INT           NULL,
    [CalledMsClassmark]                 VARCHAR (100) NULL,
    [AdditionalRoutingCategory]         INT           NULL,
    [CallOrigin]                        INT           NULL,
    [CallerByPass]                      BIT           NULL,
    [ICSUserFlag]                       VARCHAR (100) NULL,
    [UserTypeofIMSSF]                   INT           NULL,
    [OverseasFlag]                      INT           NULL,
    [ServiceAttribute]                  INT           NULL,
    [RelatedICIDValue]                  VARCHAR (100) NULL,
    [HomeZoneValue]                     INT           NULL,
    [EMAValue]                          INT           NULL,
    [CalledOfCInd]                      INT           NULL,
    [CallerOfCInd]                      INT           NULL,
    [CallerChangeOfCGI]                 VARCHAR (100) NULL,
    [UltraFlashCsfbFlag]                BIT           NULL,
    [CsfbFlag]                          BIT           NULL,
    [ISDN_BC]                           VARCHAR (100) NULL,
    [lLC]                               VARCHAR (100) NULL,
    [HLC]                               VARCHAR (100) NULL,
    [ChargePulseNumforITXTXA]           INT           NULL,
    [ChargeBandNumber]                  VARCHAR (100) NULL,
    [VoawFlag]                          BIT           NULL,
    [InputCalledNumber]                 VARCHAR (100) NULL,
    [DefaultTKNumber]                   VARCHAR (100) NULL,
    [CallerIMSI]                        VARCHAR (100) NULL,
    [AdditionalCallerNum]               VARCHAR (100) NULL,
    [FollowMeInd]                       INT           NULL,
    [ConnectedCI]                       VARCHAR (100) NULL,
    [CalledByPass]                      BIT           NULL,
    [CalledChangeOfCGI]                 VARCHAR (100) NULL,
    [RedirectingNumber]                 VARCHAR (100) NULL,
    [InitialCallAttemptFlag]            VARCHAR (100) NULL,
    [CnapResult]                        INT           NULL,
    [CnapCode]                          INT           NULL,
    [MapByPassInd]                      BIT           NULL,
    [UserProvidedCallingPartyNumber]    VARCHAR (100) NULL,
    [MSCIncomingRouteAttribute]         INT           NULL,
    [MSCOutgoingRouteAttribute]         INT           NULL,
    [SetupTimeMs]                       INT           NULL,
    [AlertingTimeMs]                    INT           NULL,
    [AnswerTimeMs]                      INT           NULL,
    [ReleaseTimeMs]                     INT           NULL,
    [OutputCalledNumber]                VARCHAR (100) NULL,
    [CalledCodecUsedFinal]              INT           NULL,
    [CallerCodecUsedFinal]              INT           NULL,
    [CallForwardInd]                    VARCHAR (100) NULL,
    [CallingNumberReceived]             VARCHAR (100) NULL,
    [CallingNumberSent]                 VARCHAR (100) NULL,
    [ClockReliability]                  BIT           NULL,
    [CMNFlag]                           INT           NULL,
    [CrankBackFrequency]                INT           NULL,
    [IdpCalledNumber]                   VARCHAR (100) NULL,
    [ReleaseCause]                      INT           NULL,
    [IsdnBasicService]                  VARCHAR (100) NULL,
    [OrgMGWMediaIP]                     VARCHAR (100) NULL,
    [OrgMGWSignalIP]                    VARCHAR (100) NULL,
    [OrgMSCSignalIP]                    VARCHAR (100) NULL,
    [TermMSCSignalIP]                   VARCHAR (100) NULL,
    [TermMGWSignalIP]                   VARCHAR (100) NULL,
    [TermMGWMediaIP]                    VARCHAR (100) NULL,
    [PCDRInfo]                          VARCHAR (100) NULL,
    [SciChargeNumber]                   VARCHAR (100) NULL,
    [SWIDTGID]                          VARCHAR (100) NULL,
    [TransmissionMediumRequirement]     INT           NULL,
    CONSTRAINT [PK_MobileCDR] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_MobileCDR_SeizureTime]
    ON [Mobile_EDR].[MobileCDR_Full]([SeizureTime] ASC);

