CREATE TYPE [TCAnal_CDR].[MappedCDRType] AS TABLE (
    [ID]                BIGINT           NULL,
    [DataSourceID]      UNIQUEIDENTIFIER NULL,
    [AttemptDateTime]   DATETIME         NULL,
    [CallingNumber]     VARCHAR (40)     NULL,
    [CalledNumber]      VARCHAR (40)     NULL,
    [CDRType]           INT              NULL,
    [DurationInSeconds] DECIMAL (20, 4)  NULL,
    [OperatorID]        BIGINT           NULL,
    [OrigCallingNumber] VARCHAR (40)     NULL,
    [OrigCalledNumber]  VARCHAR (40)     NULL,
    [CLI]               VARCHAR (40)     NULL);

