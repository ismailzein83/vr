CREATE TABLE [dbo].[Report] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [ReportDate]        DATETIME       NULL,
    [UserId]            INT            NULL,
    [ReportNumber]      VARCHAR (12)   NULL,
    [Description]       NVARCHAR (MAX) NULL,
    [ReportingStatusID] INT            CONSTRAINT [DF_Report_ReportingStatusID] DEFAULT ((1)) NOT NULL,
    [ReportID]          VARCHAR (50)   NULL,
    [SentDate]          DATETIME       NULL,
    [SentBy]            INT            NULL,
    CONSTRAINT [PK_Report] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Report_ReportingStatus] FOREIGN KEY ([ReportingStatusID]) REFERENCES [dbo].[ReportingStatus] ([ID]),
    CONSTRAINT [FK_Report_Users] FOREIGN KEY ([SentBy]) REFERENCES [dbo].[Users] ([ID])
);

