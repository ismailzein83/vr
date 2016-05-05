CREATE TABLE [Analytic].[AnalyticItemConfig] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [TableId]     INT            NOT NULL,
    [ItemType]    INT            NOT NULL,
    [Name]        VARCHAR (255)  NOT NULL,
    [Title]       NVARCHAR (255) NULL,
    [Config]      NVARCHAR (MAX) NOT NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_AnalyticItemConfig_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_AnalyticItemConfig] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_AnalyticItemConfig_NameInTable] UNIQUE NONCLUSTERED ([TableId] ASC, [Name] ASC, [ItemType] ASC)
);



