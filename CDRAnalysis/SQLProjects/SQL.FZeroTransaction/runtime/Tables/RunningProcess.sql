CREATE TABLE [runtime].[RunningProcess] (
    [ID]                INT             IDENTITY (1, 1) NOT NULL,
    [ProcessName]       NVARCHAR (1000) NOT NULL,
    [MachineName]       NVARCHAR (1000) NOT NULL,
    [StartedTime]       DATETIME        NOT NULL,
    [LastHeartBeatTime] DATETIME        NOT NULL,
    CONSTRAINT [PK_RunningProcess] PRIMARY KEY CLUSTERED ([ID] ASC)
);

