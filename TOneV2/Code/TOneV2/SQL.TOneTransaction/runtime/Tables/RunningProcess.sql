CREATE TABLE [runtime].[RunningProcess] (
    [ID]             INT             IDENTITY (1, 1) NOT NULL,
    [ProcessName]    NVARCHAR (1000) NOT NULL,
    [MachineName]    NVARCHAR (1000) NOT NULL,
    [StartedTime]    DATETIME        NOT NULL,
    [AdditionalInfo] NVARCHAR (MAX)  NULL,
    [timestamp]      ROWVERSION      NULL,
    CONSTRAINT [PK_RunningProcess] PRIMARY KEY CLUSTERED ([ID] ASC)
);



