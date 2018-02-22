CREATE TYPE [Mediation_WHS].[NokiaSiemensCDRType] AS TABLE (
    [Id]                       BIGINT       NULL,
    [BeginDate]                DATETIME     NULL,
    [EndDate]                  DATETIME     NULL,
    [DurationInSeconds]        INT          NULL,
    [CallingPartyNumber]       VARCHAR (50) NULL,
    [CalledPartyNumber]        VARCHAR (50) NULL,
    [IncomingTrunkGroupNumber] VARCHAR (50) NULL,
    [IncomingTrunkNumber]      INT          NULL,
    [OutgoingTrunkGroupNumber] VARCHAR (50) NULL,
    [OutgoingTrunkNumber]      INT          NULL,
    [CauseValue]               INT          NULL,
    [Zone]                     VARCHAR (50) NULL);

