CREATE TYPE [TCAnal_CDR].[CDRType] AS TABLE (
    [ID]                BIGINT           NULL,
    [DataSourceID]      UNIQUEIDENTIFIER NULL,
    [AttemptDateTime]   DATETIME         NULL,
    [DurationInSeconds] DECIMAL (20, 4)  NULL,
    [CalledNumber]      VARCHAR (40)     NULL,
    [CallingNumber]     VARCHAR (40)     NULL,
    [CDRType]           INT              NULL,
    [CLI]               VARCHAR (40)     NULL);

