CREATE TABLE [TCAnal_CDR].[InvalidCDR] (
    [ID]                BIGINT           NOT NULL,
    [DataSourceId]      UNIQUEIDENTIFIER NULL,
    [AttemptDateTime]   DATETIME         NULL,
    [ClientId]          NVARCHAR (40)    NULL,
    [CallingNumber]     NVARCHAR (255)   NULL,
    [CalledNumber]      NVARCHAR (255)   NULL,
    [OriginatedZoneId]  BIGINT           NULL,
    [CDRType]           INT              NULL,
    [DurationInSeconds] DECIMAL (20, 4)  NULL,
    [CallingOperatorID] BIGINT           NULL,
    [CalledOperatorID]  BIGINT           NULL,
    [OrigCallingNumber] NVARCHAR (255)   NULL,
    [OrigCalledNumber]  NVARCHAR (255)   NULL,
    [CallingNumberType] INT              NULL,
    [CreatedTime]       DATETIME         NULL,
    CONSTRAINT [PK__InvalidC__3214EC275629CD9C] PRIMARY KEY CLUSTERED ([ID] ASC)
);



