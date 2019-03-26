CREATE TABLE [TCAnal_CDR].[CDR] (
    [ID]                BIGINT           NOT NULL,
    [DataSourceId]      UNIQUEIDENTIFIER NULL,
    [AttemptDateTime]   DATETIME         NULL,
    [DurationInSeconds] DECIMAL (20, 4)  NULL,
    [CallingNumber]     VARCHAR (40)     NULL,
    [CalledNumber]      VARCHAR (40)     NULL,
    [CDRType]           INT              NULL,
    [LastModifiedBy]    INT              NULL,
    [CreatedBy]         INT              NULL,
    [LastModifiedTime]  DATETIME         NULL,
    [CreatedTime]       DATETIME         NULL,
    [timestamp]         ROWVERSION       NULL,
    CONSTRAINT [PK__CDR__3214EC272C3393D0] PRIMARY KEY CLUSTERED ([ID] ASC)
);

