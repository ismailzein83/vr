CREATE TABLE [common].[ExtensionConfiguration] (
    [OldID]       INT              NOT NULL,
    [Name]        NVARCHAR (255)   NULL,
    [Title]       NVARCHAR (255)   NULL,
    [ConfigType]  NVARCHAR (255)   NULL,
    [Settings]    NVARCHAR (MAX)   NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_ExtensionConfiguration_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL,
    [ID]          UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_ExtensionConfiguration] PRIMARY KEY CLUSTERED ([OldID] ASC)
);












GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ExtensionConfiguration]
    ON [common].[ExtensionConfiguration]([ID] ASC);

