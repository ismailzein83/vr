CREATE TABLE [dbo].[LoggedActions] (
    [ID]           INT           IDENTITY (1, 1) NOT NULL,
    [ActionTypeID] INT           NOT NULL,
    [LogDate]      DATETIME2 (0) CONSTRAINT [DF_LoggedActions_LogDate] DEFAULT (getdate()) NOT NULL,
    [ActionBy]     INT           NOT NULL,
    CONSTRAINT [PK_LoggedActions] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_LoggedActions_Actions] FOREIGN KEY ([ActionTypeID]) REFERENCES [dbo].[ActionTypes] ([ID]),
    CONSTRAINT [FK_LoggedActions_Users] FOREIGN KEY ([ActionBy]) REFERENCES [dbo].[Users] ([ID])
);

