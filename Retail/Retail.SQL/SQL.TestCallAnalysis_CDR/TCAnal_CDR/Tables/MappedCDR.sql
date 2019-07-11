CREATE TABLE [TCAnal_CDR].[MappedCDR] (
    [ID]                BIGINT           NOT NULL,
    [DataSourceId]      UNIQUEIDENTIFIER NULL,
    [ClientId]          NVARCHAR (40)    NULL,
    [CallingNumber]     VARCHAR (40)     NULL,
    [CalledNumber]      VARCHAR (40)     NULL,
    [OriginatedZoneId]  BIGINT           NULL,
    [CDRType]           INT              NULL,
    [AttemptDateTime]   DATETIME         NULL,
    [DurationInSeconds] DECIMAL (20, 4)  NULL,
    [CallingOperatorID] BIGINT           NULL,
    [CalledOperatorID]  BIGINT           NULL,
    [OrigCallingNumber] VARCHAR (40)     NULL,
    [OrigCalledNumber]  VARCHAR (40)     NULL,
    [CallingNumberType] INT              NULL,
    [IsCorrelated]      BIT              NULL,
    [CreatedTime]       DATETIME         NULL,
    CONSTRAINT [PK__MappedCD__3214EC271BFD2C07] PRIMARY KEY CLUSTERED ([ID] ASC)
);









