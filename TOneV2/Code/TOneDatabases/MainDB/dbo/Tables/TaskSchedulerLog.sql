CREATE TABLE [dbo].[TaskSchedulerLog] (
    [LogID]              INT          IDENTITY (1, 1) NOT NULL,
    [CheckerType]        VARCHAR (50) NULL,
    [ProcessingStart]    DATETIME     NULL,
    [ProcessingEnd]      DATETIME     NULL,
    [FromTimeRange]      DATETIME     NULL,
    [ToTimeRange]        DATETIME     NULL,
    [GeneratedTaskCount] INT          NULL,
    CONSTRAINT [PK_TaskSchedulerLog] PRIMARY KEY CLUSTERED ([LogID] ASC)
);

