CREATE TABLE [dbo].[CDR] (
    [ID]                 INT             IDENTITY (1, 1) NOT NULL,
    [SourceID]           INT             NOT NULL,
    [Reference]          VARCHAR (100)   NULL,
    [ConnectDateTime]    DATETIME        NULL,
    [DisconnectDateTime] DATETIME        NULL,
    [DurationInSeconds]  NUMERIC (13, 7) NULL,
    [IN_TRUNK]           VARCHAR (100)   NULL,
    [OUT_TRUNK]          VARCHAR (100)   NULL,
    [CGPN]               VARCHAR (100)   NULL,
    [CDPN]               VARCHAR (100)   NULL,
    [IGNORE]             INT             NULL,
    [in_type]            VARCHAR (100)   NULL,
    [out_type]           VARCHAR (100)   NULL,
    [switch]             VARCHAR (100)   NULL,
    [A_temp]             VARCHAR (100)   NULL,
    [B_temp]             VARCHAR (100)   NULL,
    [IsNormalized]       INT             CONSTRAINT [DF_GeneratedCalls_IsNormalized] DEFAULT ((0)) NULL,
    [ImportID]           INT             NULL,
    CONSTRAINT [PK_CDR] PRIMARY KEY NONCLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80, STATISTICS_NORECOMPUTE = ON)
);


GO
CREATE NONCLUSTERED INDEX [I_CDR_B_Temp]
    ON [dbo].[CDR]([B_temp] ASC) WITH (FILLFACTOR = 80, STATISTICS_NORECOMPUTE = ON);


GO
CREATE NONCLUSTERED INDEX [I_CDR_A_temp]
    ON [dbo].[CDR]([A_temp] ASC) WITH (FILLFACTOR = 80, STATISTICS_NORECOMPUTE = ON);


GO
CREATE NONCLUSTERED INDEX [CGPN]
    ON [dbo].[CDR]([CGPN] ASC) WITH (FILLFACTOR = 80, STATISTICS_NORECOMPUTE = ON);


GO
CREATE NONCLUSTERED INDEX [I_CDR_CDPN]
    ON [dbo].[CDR]([CDPN] ASC) WITH (FILLFACTOR = 80, STATISTICS_NORECOMPUTE = ON);


GO
CREATE UNIQUE CLUSTERED INDEX [I_CDR_ID]
    ON [dbo].[CDR]([ID] ASC) WITH (FILLFACTOR = 80, STATISTICS_NORECOMPUTE = ON);

