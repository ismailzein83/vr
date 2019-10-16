﻿CREATE TABLE [Mediation_ICX].[NokiaSiemensCDR] (
    [Id]                          BIGINT           IDENTITY (1, 1) NOT NULL,
    [BeginDate]                   DATETIME         NULL,
    [EndDate]                     DATETIME         NULL,
    [DurationInSeconds]           INT              NULL,
    [CallingPartyNumber]          VARCHAR (50)     NULL,
    [CalledPartyNumber]           VARCHAR (50)     NULL,
    [CallingNADI]                 INT              NULL,
    [CalledNADI]                  INT              NULL,
    [CallingNPI]                  INT              NULL,
    [CalledNPI]                   INT              NULL,
    [IncomingTrunkGroupNumber]    VARCHAR (50)     NULL,
    [IncomingTrunkNumber]         INT              NULL,
    [OutgoingTrunkGroupNumber]    VARCHAR (50)     NULL,
    [OutgoingTrunkNumber]         INT              NULL,
    [IncomingTrunkGroupNumberCIC] VARCHAR (50)     NULL,
    [OutgoingTrunkGroupNumberCIC] VARCHAR (50)     NULL,
    [CauseValue]                  INT              NULL,
    [Zone]                        INT              NULL,
    [ConnectionIdentification]    BIGINT           NULL,
    [CodingStandard]              INT              NULL,
    [Location]                    INT              NULL,
    [FileName]                    VARCHAR (255)    NULL,
    [DataSourceId]                UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_NokiaSiemensCDR] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_NokiaSiemensCDR_BeginDate]
    ON [Mediation_ICX].[NokiaSiemensCDR]([BeginDate] ASC);

