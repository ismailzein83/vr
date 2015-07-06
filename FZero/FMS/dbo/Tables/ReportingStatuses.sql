CREATE TABLE [dbo].[ReportingStatuses] (
    [ID]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_ReportingStatuses] PRIMARY KEY CLUSTERED ([ID] ASC)
);

