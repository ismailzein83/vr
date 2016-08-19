CREATE TABLE [runtime].[RuntimeManager] (
    [ID]                INT              NOT NULL,
    [InstanceID]        UNIQUEIDENTIFIER NOT NULL,
    [ServiceURL]        VARCHAR (255)    NOT NULL,
    [LastHeartBeatTime] DATETIME         NOT NULL,
    CONSTRAINT [PK_RunningProcessManager] PRIMARY KEY CLUSTERED ([ID] ASC)
);

