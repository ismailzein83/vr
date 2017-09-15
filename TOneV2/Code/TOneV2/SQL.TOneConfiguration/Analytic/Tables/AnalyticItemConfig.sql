CREATE TABLE [Analytic].[AnalyticItemConfig] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [TableId]     UNIQUEIDENTIFIER NOT NULL,
    [ItemType]    INT              NOT NULL,
    [Name]        VARCHAR (255)    NOT NULL,
    [Title]       NVARCHAR (255)   NULL,
    [Config]      NVARCHAR (MAX)   NOT NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_AnalyticItemConfig_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_AnalyticItemConfig_1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_AnalyticItemConfig_AnalyticTable] FOREIGN KEY ([TableId]) REFERENCES [Analytic].[AnalyticTable] ([ID])
);














GO
CREATE NONCLUSTERED INDEX [IX_AnalyticItemConfig_NameInTable]
    ON [Analytic].[AnalyticItemConfig]([TableId] ASC, [Name] ASC, [ItemType] ASC);

