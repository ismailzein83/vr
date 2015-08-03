﻿CREATE TABLE [dbo].[Quer] (
    [CDRID]                   BIGINT          NOT NULL,
    [SwitchID]                TINYINT         NOT NULL,
    [IDonSwitch]              BIGINT          NULL,
    [Tag]                     VARCHAR (100)   NULL,
    [AttemptDateTime]         DATETIME        NULL,
    [AlertDateTime]           DATETIME        NULL,
    [ConnectDateTime]         DATETIME        NULL,
    [DisconnectDateTime]      DATETIME        NULL,
    [DurationInSeconds]       DECIMAL (13, 4) NULL,
    [IN_TRUNK]                VARCHAR (5)     NULL,
    [IN_CIRCUIT]              SMALLINT        NULL,
    [IN_CARRIER]              VARCHAR (100)   NULL,
    [IN_IP]                   VARCHAR (21)    NULL,
    [OUT_TRUNK]               VARCHAR (5)     NULL,
    [OUT_CIRCUIT]             SMALLINT        NULL,
    [OUT_CARRIER]             VARCHAR (100)   NULL,
    [OUT_IP]                  VARCHAR (21)    NULL,
    [CGPN]                    VARCHAR (40)    NULL,
    [CDPN]                    VARCHAR (40)    NULL,
    [CAUSE_FROM_RELEASE_CODE] VARCHAR (40)    NULL,
    [CAUSE_FROM]              VARCHAR (20)    NULL,
    [CAUSE_TO_RELEASE_CODE]   VARCHAR (40)    NULL,
    [CAUSE_TO]                VARCHAR (20)    NULL,
    [Extra_Fields]            VARCHAR (255)   NULL,
    [IsRerouted]              CHAR (1)        NULL
);

