CREATE TABLE [runtime].[RuntimeManager] (
    [ID]         INT              NOT NULL,
    [InstanceID] UNIQUEIDENTIFIER NOT NULL,
    [TakenTime]  DATETIME         NULL,
    CONSTRAINT [PK_RunningProcessManager] PRIMARY KEY CLUSTERED ([ID] ASC)
);





