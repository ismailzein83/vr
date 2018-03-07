CREATE TABLE [Mediation_ICX].[NokiaSiemensCDR] (
    [Id]                       BIGINT        NOT NULL,
    [BeginDate]                DATETIME      NULL,
    [EndDate]                  DATETIME      NULL,
    [DurationInSeconds]        INT           NULL,
    [CallingPartyNumber]       VARCHAR (50)  NULL,
    [CalledPartyNumber]        VARCHAR (50)  NULL,
    [IncomingTrunkGroupNumber] VARCHAR (50)  NULL,
    [IncomingTrunkNumber]      INT           NULL,
    [OutgoingTrunkGroupNumber] VARCHAR (50)  NULL,
    [OutgoingTrunkNumber]      INT           NULL,
    [CauseValue]               INT           NULL,
    [Zone]                     INT           NULL,
    [FileName]                 VARCHAR (255) NULL,
    CONSTRAINT [PK_NokiaSiemensCDR] PRIMARY KEY CLUSTERED ([Id] ASC)
);







