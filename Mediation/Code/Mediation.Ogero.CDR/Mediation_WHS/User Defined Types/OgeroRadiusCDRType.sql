﻿CREATE TYPE [Mediation_WHS].[OgeroRadiusCDRType] AS TABLE (
    [Id]                     BIGINT       NULL,
    [ComputerName]           VARCHAR (50) NULL,
    [ServiceName]            VARCHAR (50) NULL,
    [RecordDate]             DATE         NULL,
    [RecordTime]             TIME (3)     NULL,
    [PacketType]             INT          NULL,
    [UserName]               VARCHAR (50) NULL,
    [FullyQualifiedUserName] VARCHAR (50) NULL,
    [CalledStationID]        VARCHAR (50) NULL,
    [CallingStationID]       VARCHAR (50) NULL,
    [CallbackNumber]         VARCHAR (50) NULL,
    [FramedIPAddress]        VARCHAR (50) NULL,
    [NASIdentifier]          VARCHAR (50) NULL,
    [NASIPAddress]           VARCHAR (50) NULL,
    [NASPort]                VARCHAR (50) NULL,
    [ClientVendor]           VARCHAR (50) NULL,
    [ClientIPAddress]        VARCHAR (50) NULL,
    [ClientFriendlyName]     VARCHAR (50) NULL,
    [EventTimestamp]         DATETIME     NULL,
    [PortLimit]              VARCHAR (50) NULL,
    [NASPortType]            INT          NULL,
    [ConnectInfo]            VARCHAR (50) NULL,
    [FramedProtocol]         INT          NULL,
    [ServiceType]            INT          NULL,
    [AuthenticationType]     INT          NULL,
    [NPPolicyName]           VARCHAR (50) NULL,
    [ReasonCode]             INT          NULL,
    [Class]                  VARCHAR (50) NULL,
    [SessionTimeout]         INT          NULL,
    [IdleTimeout]            INT          NULL,
    [TerminationAction]      INT          NULL,
    [EAPFriendlyName]        VARCHAR (50) NULL,
    [AcctStatusType]         INT          NULL,
    [AcctDelayTime]          INT          NULL,
    [AcctInputOctets]        BIGINT       NULL,
    [AcctOutputOctets]       BIGINT       NULL,
    [AcctSessionId]          VARCHAR (50) NULL,
    [AcctAuthentic]          INT          NULL,
    [AcctSessionTime]        VARCHAR (50) NULL,
    [AcctInputPackets]       BIGINT       NULL,
    [AcctOutputPackets]      BIGINT       NULL,
    [AcctTerminateCause]     INT          NULL,
    [AcctMultiSsnID]         INT          NULL,
    [AcctLinkCount]          INT          NULL,
    [AcctInterimInterval]    INT          NULL,
    [TunnelType]             VARCHAR (50) NULL,
    [TunnelMediumType]       VARCHAR (50) NULL,
    [TunnelClientEndpt]      VARCHAR (50) NULL,
    [TunnelServerEndpt]      VARCHAR (50) NULL,
    [AcctTunnelConnection]   VARCHAR (50) NULL,
    [TunnelPvtGroupID]       VARCHAR (50) NULL,
    [TunnelAssignmentID]     VARCHAR (50) NULL,
    [TunnelPreference]       INT          NULL,
    [MSAcctAuthType]         INT          NULL,
    [MSAcctEAPType]          INT          NULL,
    [MSRASVersion]           VARCHAR (50) NULL,
    [MSRASVendor]            INT          NULL,
    [MSCHAPError]            VARCHAR (50) NULL,
    [MSCHAPDomain]           VARCHAR (50) NULL,
    [MSMPPEEncryptionTypes]  INT          NULL,
    [MSMPPEEncryptionPolicy] INT          NULL,
    [FileName]               VARCHAR (50) NULL);

