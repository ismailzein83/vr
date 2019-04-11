CREATE TABLE [TCAnal_CDR].[MappedCDR] (
    [ID]                BIGINT           NOT NULL,
    [DataSourceId]      UNIQUEIDENTIFIER NULL,
    [AttemptDateTime]   DATETIME         NULL,
    [CallingNumber]     VARCHAR (40)     NULL,
    [CalledNumber]      VARCHAR (40)     NULL,
    [CDRType]           INT              NULL,
    [DurationInSeconds] DECIMAL (20, 4)  NULL,
    [OperatorID]        BIGINT           NULL,
    [OrigCallingNumber] VARCHAR (40)     NULL,
    [OrigCalledNumber]  VARCHAR (40)     NULL,
    [CallingNumberType] INT              NULL,
    [CalledNumberType]  INT              NULL,
    [IsCorrelated]      BIT              NULL,
    [CreatedTime]       DATETIME         NULL,
    [timestamp]         ROWVERSION       NULL,
    CONSTRAINT [PK__MappedCD__3214EC271BFD2C07] PRIMARY KEY CLUSTERED ([ID] ASC)
);



