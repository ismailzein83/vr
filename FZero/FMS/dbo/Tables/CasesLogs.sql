CREATE TABLE [dbo].[CasesLogs] (
    [ID]                       INT      IDENTITY (1, 1) NOT NULL,
    [ChangeTypeID]             INT      NOT NULL,
    [ReportingStatusID]        INT      NULL,
    [StatusID]                 INT      NULL,
    [MobileOperatorFeedbackID] INT      NULL,
    [UpdatedOn]                DATETIME NOT NULL,
    [GeneratedCallID]          INT      NOT NULL,
    CONSTRAINT [PK_ReportingStatusLog] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_CasesLogs_ChangeTypes] FOREIGN KEY ([ChangeTypeID]) REFERENCES [dbo].[ChangeTypes] ([ID]),
    CONSTRAINT [FK_CasesLogs_MobileOperatorFeedbacks] FOREIGN KEY ([MobileOperatorFeedbackID]) REFERENCES [dbo].[MobileOperatorFeedbacks] ([ID]),
    CONSTRAINT [FK_CasesLogs_ReportingStatuses] FOREIGN KEY ([ReportingStatusID]) REFERENCES [dbo].[ReportingStatuses] ([ID]),
    CONSTRAINT [FK_CasesLogs_Statuses] FOREIGN KEY ([StatusID]) REFERENCES [dbo].[Statuses] ([ID])
);

