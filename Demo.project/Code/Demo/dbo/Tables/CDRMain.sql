CREATE TABLE [dbo].[CDRMain] (
    [Attempt]           DATETIME      NOT NULL,
    [CDPN]              NVARCHAR (50) NULL,
    [CGPN]              NVARCHAR (50) NULL,
    [OutTrunk]          NVARCHAR (50) NULL,
    [InTrunk]           NVARCHAR (50) NULL,
    [ServiceTypeID]     INT           NULL,
    [Direction]         INT           NULL,
    [DurationInSeconds] DECIMAL (18)  NULL,
    [Alert]             DATETIME      NULL,
    [Connect]           DATETIME      NULL,
    [Disconnect]        DATETIME      NULL,
    [PortOut]           NVARCHAR (50) NULL,
    [PortIn]            NVARCHAR (50) NULL,
    [ReleaseCode]       NVARCHAR (50) NULL,
    [ReleaseSource]     NVARCHAR (50) NULL,
    [DataSourceID]      INT           NOT NULL,
    [CDRType]           INT           NOT NULL,
    [OperatorID]        INT           NULL,
    [ZoneID]            INT           NULL,
    [Code]              VARCHAR (20)  NULL
);


GO
CREATE CLUSTERED INDEX [IX_CDR_Attempt]
    ON [dbo].[CDRMain]([Attempt] ASC);

