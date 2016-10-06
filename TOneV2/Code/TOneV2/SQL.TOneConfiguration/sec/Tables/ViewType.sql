CREATE TABLE [sec].[ViewType] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [OldID]       INT              NULL,
    [Name]        VARCHAR (255)    NULL,
    [Title]       VARCHAR (255)    NULL,
    [Details]     NVARCHAR (MAX)   NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_sec.ViewType_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_sec.ViewType] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_ViewType] UNIQUE NONCLUSTERED ([Name] ASC)
);





