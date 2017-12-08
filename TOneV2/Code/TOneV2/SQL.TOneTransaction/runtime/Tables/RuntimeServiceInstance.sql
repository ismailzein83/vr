CREATE TABLE [runtime].[RuntimeServiceInstance] (
    [ID]                  UNIQUEIDENTIFIER NOT NULL,
    [ServiceTypeID]       INT              NOT NULL,
    [ProcessID]           INT              NOT NULL,
    [ServiceInstanceInfo] NVARCHAR (MAX)   NULL,
    [timestamp]           ROWVERSION       NULL,
    [CreatedTime]         DATETIME         CONSTRAINT [DF_RuntimeService_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_RuntimeService] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_RuntimeServiceInstance_RunningProcess] FOREIGN KEY ([ProcessID]) REFERENCES [runtime].[RunningProcess] ([ID])
);



