CREATE TABLE [dbo].[ControlTable] (
    [ID]                     INT      IDENTITY (1, 1) NOT NULL,
    [OperationTypeID]        INT      NOT NULL,
    [StartedDateTime]        DATETIME NOT NULL,
    [FinishedDateTime]       DATETIME NULL,
    [RowsPassed]             INT      CONSTRAINT [DF_ActionLog_RowsPassed] DEFAULT ((0)) NULL,
    [TotalRows]              INT      CONSTRAINT [DF_ActionLog_TotalRows] DEFAULT ((0)) NULL,
    [StartID]                INT      NULL,
    [EndID]                  INT      NULL,
    [StartDate]              DATETIME NULL,
    [EndDate]                DATETIME NULL,
    [StartingUnitdate]       DATETIME NULL,
    [EndingUnitdate]         DATETIME NULL,
    [Lastid]                 INT      NULL,
    [PeriodId]               INT      NULL,
    [NumberOfProfileRecords] INT      NULL,
    [StrategyId]             INT      NULL,
    [NumberOfCalls]          INT      NULL,
    CONSTRAINT [PK_ActionLog] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ControlTable_OperationType] FOREIGN KEY ([OperationTypeID]) REFERENCES [dbo].[OperationType] ([ID])
);

