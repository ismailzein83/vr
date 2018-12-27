CREATE TABLE [Analytic].[AnalyticReport] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [UserID]           INT              NULL,
    [Name]             NVARCHAR (255)   NULL,
    [AccessType]       INT              NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_RealTimeReport_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_AnalyticReport_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_RealTimeReport] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_RealTimeReport] UNIQUE NONCLUSTERED ([Name] ASC, [UserID] ASC)
);







