﻿CREATE TABLE [Mediation_WHS].[MobilisBadSMS] (
    [Id]                       BIGINT           NULL,
    [SMID]                     VARCHAR (50)     NULL,
    [OriginalAddress]          VARCHAR (50)     NULL,
    [DestAddr]                 VARCHAR (50)     NULL,
    [MOMSCAddr]                VARCHAR (50)     NULL,
    [ScAddr]                   VARCHAR (50)     NULL,
    [EsmClass]                 VARCHAR (50)     NULL,
    [PriorityLevel]            VARCHAR (50)     NULL,
    [RD]                       VARCHAR (50)     NULL,
    [ReplyPath]                VARCHAR (50)     NULL,
    [UDHI]                     VARCHAR (50)     NULL,
    [SRR]                      VARCHAR (50)     NULL,
    [MR]                       VARCHAR (50)     NULL,
    [PID]                      VARCHAR (50)     NULL,
    [DCS]                      VARCHAR (50)     NULL,
    [ScheduleTime]             DATETIME         NULL,
    [ExpireTime]               DATETIME         NULL,
    [DefaultID]                VARCHAR (50)     NULL,
    [UDL]                      VARCHAR (50)     NULL,
    [SMType]                   VARCHAR (50)     NULL,
    [SMSubmissionResult]       VARCHAR (50)     NULL,
    [FCS]                      VARCHAR (50)     NULL,
    [WriteTime]                DATETIME         NULL,
    [Message]                  VARCHAR (MAX)    NULL,
    [ServiceType]              VARCHAR (50)     NULL,
    [PPSUser]                  VARCHAR (50)     NULL,
    [OrgAccount]               VARCHAR (50)     NULL,
    [CalledServiceFlag]        VARCHAR (50)     NULL,
    [RawOrgAddress]            VARCHAR (50)     NULL,
    [RawDestAddress]           VARCHAR (50)     NULL,
    [SubmitMultiID]            VARCHAR (50)     NULL,
    [OriginalGroup]            VARCHAR (50)     NULL,
    [OrgCommandID]             VARCHAR (50)     NULL,
    [RawOrgTON]                VARCHAR (50)     NULL,
    [RawOrgNPI]                VARCHAR (50)     NULL,
    [RawDestTON]               VARCHAR (50)     NULL,
    [RawDestNPI]               VARCHAR (50)     NULL,
    [MOMSCAddrType]            VARCHAR (50)     NULL,
    [MOMSCTON]                 VARCHAR (50)     NULL,
    [MOMSCNPI]                 VARCHAR (50)     NULL,
    [DestNetType]              VARCHAR (50)     NULL,
    [DestIFType]               VARCHAR (50)     NULL,
    [TLVsDataLen]              VARCHAR (50)     NULL,
    [TLVsData]                 VARCHAR (50)     NULL,
    [IFForward]                VARCHAR (50)     NULL,
    [UDEncryptType]            VARCHAR (50)     NULL,
    [OrgOPID]                  VARCHAR (50)     NULL,
    [DestOPID]                 VARCHAR (50)     NULL,
    [QueryOrgOPIDResult]       VARCHAR (50)     NULL,
    [QueryDestOPIDResult]      VARCHAR (50)     NULL,
    [NetworkErrorCode]         VARCHAR (50)     NULL,
    [OrgOCSID]                 VARCHAR (50)     NULL,
    [DestOCSID]                VARCHAR (50)     NULL,
    [DestAccount]              VARCHAR (50)     NULL,
    [MessageType]              VARCHAR (50)     NULL,
    [SRICallingSCCP]           VARCHAR (50)     NULL,
    [MTSubmitCallingSCCP]      VARCHAR (50)     NULL,
    [OrgIMSIAddr]              VARCHAR (50)     NULL,
    [DestIMSIAddr]             VARCHAR (50)     NULL,
    [MOSubmitTime]             DATETIME         NULL,
    [BillingIdentification]    VARCHAR (50)     NULL,
    [AntiSpammingCheckResult]  VARCHAR (50)     NULL,
    [FilterdAVP]               VARCHAR (50)     NULL,
    [IsDestNative]             VARCHAR (50)     NULL,
    [SMContentKeyword]         VARCHAR (50)     NULL,
    [SMMR]                     VARCHAR (50)     NULL,
    [SMRN]                     VARCHAR (50)     NULL,
    [SMMN]                     VARCHAR (50)     NULL,
    [SMSN]                     VARCHAR (50)     NULL,
    [ServiceControlResultCode] VARCHAR (50)     NULL,
    [RealTimeRated]            VARCHAR (50)     NULL,
    [ServiceCtrlResult]        VARCHAR (50)     NULL,
    [CgiAddr]                  VARCHAR (50)     NULL,
    [IMEIAddr]                 VARCHAR (50)     NULL,
    [LastResort]               VARCHAR (50)     NULL,
    [OriginalValidityPeriod]   VARCHAR (50)     NULL,
    [OriginalSubmissionTime]   DATETIME         NULL,
    [SMBufferingForAntiSpam]   VARCHAR (50)     NULL,
    [AntiSpoofingResult]       VARCHAR (50)     NULL,
    [RealMSCAddr]              VARCHAR (50)     NULL,
    [CalledCellID]             VARCHAR (50)     NULL,
    [SpecialServiceIndicator]  VARCHAR (50)     NULL,
    [FileName]                 VARCHAR (255)    NULL,
    [DataSourceId]             UNIQUEIDENTIFIER NULL
);
