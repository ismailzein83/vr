CREATE TABLE [queue].[QueueActivatorInstance] (
    [ActivatorID]   UNIQUEIDENTIFIER NOT NULL,
    [ProcessID]     INT              NOT NULL,
    [ActivatorType] INT              NOT NULL,
    [ServiceURL]    VARCHAR (255)    NULL,
    CONSTRAINT [PK_QueueActivator] PRIMARY KEY CLUSTERED ([ActivatorID] ASC)
);

