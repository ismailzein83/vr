CREATE TYPE [TOneWhS_CDR].[CDRType] AS TABLE (
    [CDRId]                   BIGINT          NULL,
    [SwitchID]                INT             NULL,
    [IDonSwitch]              BIGINT          NULL,
    [Tag]                     VARCHAR (100)   NULL,
    [AttemptDateTime]         DATETIME        NULL,
    [AlertDateTime]           DATETIME        NULL,
    [ConnectDateTime]         DATETIME        NULL,
    [DisconnectDateTime]      DATETIME        NULL,
    [DurationInSeconds]       DECIMAL (20, 4) NULL,
    [IN_TRUNK]                VARCHAR (50)    NULL,
    [IN_CIRCUIT]              BIGINT          NULL,
    [IN_CARRIER]              VARCHAR (100)   NULL,
    [IN_IP]                   VARCHAR (42)    NULL,
    [OUT_TRUNK]               VARCHAR (50)    NULL,
    [OUT_CIRCUIT]             BIGINT          NULL,
    [OUT_CARRIER]             VARCHAR (100)   NULL,
    [OUT_IP]                  VARCHAR (42)    NULL,
    [CGPN]                    VARCHAR (40)    NULL,
    [CDPN]                    VARCHAR (40)    NULL,
    [CAUSE_FROM_RELEASE_CODE] VARCHAR (50)    NULL,
    [CAUSE_FROM]              VARCHAR (10)    NULL,
    [CAUSE_TO_RELEASE_CODE]   VARCHAR (50)    NULL,
    [CAUSE_TO]                VARCHAR (10)    NULL,
    [IsRerouted]              BIT             NULL,
    [CDPNOut]                 VARCHAR (50)    NULL,
    [SIP]                     VARCHAR (100)   NULL,
    [CDPNIn]                  VARCHAR (50)    NULL);





