CREATE TABLE [runtime].[RuntimeNodeState] (
    [RuntimeNodeID]        UNIQUEIDENTIFIER NOT NULL,
    [InstanceID]           UNIQUEIDENTIFIER NOT NULL,
    [MachineName]          NVARCHAR (1000)  NOT NULL,
    [OSProcessID]          INT              NOT NULL,
    [OSProcessName]        NVARCHAR (1000)  NOT NULL,
    [ServiceURL]           VARCHAR (255)    NOT NULL,
    [StartedTime]          DATETIME         NOT NULL,
    [LastHeartBeatTime]    DATETIME         NOT NULL,
    [CPUUsage]             DECIMAL (6, 2)   NULL,
    [AvailableRAM]         DECIMAL (24, 4)  NULL,
    [DiskInfos]            NVARCHAR (MAX)   NULL,
    [NbOfEnabledProcesses] INT              NULL,
    [NbOfRunningProcesses] INT              NULL,
    CONSTRAINT [PK_RuntimeNodeState] PRIMARY KEY CLUSTERED ([RuntimeNodeID] ASC)
);



