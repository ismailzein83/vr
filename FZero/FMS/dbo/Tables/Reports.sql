CREATE TABLE [dbo].[Reports] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [ReportID]            VARCHAR (50)   NOT NULL,
    [SentDateTime]        DATETIME2 (0)  NOT NULL,
    [ApplicationUserID]   INT            NULL,
    [RecommendedActionID] INT            NOT NULL,
    [RecommendedAction]   VARCHAR (3000) NULL,
    CONSTRAINT [PK_Reports] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Reports_ApplicationUsers] FOREIGN KEY ([ApplicationUserID]) REFERENCES [dbo].[ApplicationUsers] ([ID]),
    CONSTRAINT [FK_Reports_RecommendedActions] FOREIGN KEY ([RecommendedActionID]) REFERENCES [dbo].[RecommendedActions] ([ID])
);

