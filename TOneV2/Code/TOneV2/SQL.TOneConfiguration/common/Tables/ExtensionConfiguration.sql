CREATE TABLE [common].[ExtensionConfiguration] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NULL,
    [Title]       NVARCHAR (255)   NULL,
    [ConfigType]  NVARCHAR (255)   NULL,
    [Settings]    NVARCHAR (MAX)   NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_ExtensionConfiguration_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_ExtensionConfiguration_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);
















GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ExtensionConfiguration]
    ON [common].[ExtensionConfiguration]([ID] ASC);

