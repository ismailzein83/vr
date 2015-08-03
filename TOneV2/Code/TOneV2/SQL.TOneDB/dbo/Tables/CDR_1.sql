CREATE TABLE [dbo].[CDR] (
    [CDRID]                   BIGINT          IDENTITY (1, 1) NOT NULL,
    [SwitchID]                TINYINT         NOT NULL,
    [IDonSwitch]              BIGINT          NULL,
    [Tag]                     VARCHAR (100)   NULL,
    [AttemptDateTime]         DATETIME        NULL,
    [AlertDateTime]           DATETIME        NULL,
    [ConnectDateTime]         DATETIME        NULL,
    [DisconnectDateTime]      DATETIME        NULL,
    [DurationInSeconds]       NUMERIC (13, 4) NULL,
    [IN_TRUNK]                VARCHAR (50)    NULL,
    [IN_CIRCUIT]              BIGINT          NULL,
    [IN_CARRIER]              VARCHAR (100)   NULL,
    [IN_IP]                   VARCHAR (42)    NULL,
    [OUT_TRUNK]               VARCHAR (50)    NULL,
    [OUT_CIRCUIT]             SMALLINT        NULL,
    [OUT_CARRIER]             VARCHAR (100)   NULL,
    [OUT_IP]                  VARCHAR (42)    NULL,
    [CGPN]                    VARCHAR (40)    NULL,
    [CDPN]                    VARCHAR (40)    NULL,
    [CAUSE_FROM_RELEASE_CODE] VARCHAR (50)    NULL,
    [CAUSE_FROM]              VARCHAR (10)    NULL,
    [CAUSE_TO_RELEASE_CODE]   VARCHAR (50)    NULL,
    [CAUSE_TO]                VARCHAR (10)    NULL,
    [Extra_Fields]            VARCHAR (255)   NULL,
    [IsRerouted]              CHAR (1)        CONSTRAINT [DF_CDR_Is_Rerouted] DEFAULT ('N') NULL,
    [CDPNOut]                 VARCHAR (50)    NULL,
    [SIP]                     VARCHAR (100)   CONSTRAINT [DF_CDR_SIP] DEFAULT ('') NULL,
    CONSTRAINT [PK_CDR] PRIMARY KEY CLUSTERED ([CDRID] ASC) ON [TOne_CDR]
);


GO
CREATE NONCLUSTERED INDEX [IX_CDR_AttemptDateTime]
    ON [dbo].[CDR]([AttemptDateTime] ASC)
    ON [TOne_Index];

