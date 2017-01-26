CREATE TABLE [common].[ExtensionConfiguration_Temp] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NULL,
    [Title]       NVARCHAR (255)   NULL,
    [ConfigType]  NVARCHAR (255)   NULL,
    [Settings]    NVARCHAR (MAX)   NULL,
    [CreatedTime] DATETIME         NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_ExtensionConfiguration_Temp] PRIMARY KEY CLUSTERED ([ID] ASC)
);

