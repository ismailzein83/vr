CREATE TABLE [TOneWhS_CDR].[CDR] (
    [ID]                BIGINT        NULL,
    [Attempt]           DATETIME      NULL,
    [InCarrier]         NVARCHAR (50) NULL,
    [InTrunk]           NVARCHAR (50) NULL,
    [CDPN]              NVARCHAR (50) NULL,
    [OutTrunk]          NVARCHAR (50) NULL,
    [OutCarrier]        NVARCHAR (50) NULL,
    [DurationInSeconds] INT           NULL,
    [Alert]             DATETIME      NULL,
    [Connect]           DATETIME      NULL,
    [Disconnect]        DATETIME      NULL,
    [CGPN]              NVARCHAR (50) NULL,
    [PortOut]           NVARCHAR (50) NULL,
    [PortIn]            NVARCHAR (50) NULL,
    [ReleaseCode]       NVARCHAR (50) NULL,
    [ReleaseSource]     NVARCHAR (50) NULL,
    [SwitchID]          INT           NULL
);


GO
CREATE CLUSTERED INDEX [IX_CDR_Attempt]
    ON [TOneWhS_CDR].[CDR]([Attempt] DESC);

