CREATE TABLE [RouteSync].[SwitchCommandLog] (
    [ID]                BIGINT        IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceID] BIGINT        NOT NULL,
    [SwitchId]          VARCHAR (20)  NOT NULL,
    [Command]           VARCHAR (MAX) NULL,
    [Response]          VARCHAR (MAX) NULL,
    [EntryTime]         DATETIME      CONSTRAINT [DF_SwitchCommandLog_EntryTime] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_SwitchCommandLog] PRIMARY KEY CLUSTERED ([ID] ASC)
);

