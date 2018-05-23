CREATE TABLE [queue].[ExecutionControl] (
    [ID]       INT NOT NULL,
    [IsPaused] BIT NULL,
    CONSTRAINT [PK_ExecutionControl] PRIMARY KEY CLUSTERED ([ID] ASC)
);

