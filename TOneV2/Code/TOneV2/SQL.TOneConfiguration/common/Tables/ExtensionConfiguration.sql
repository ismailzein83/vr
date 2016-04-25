CREATE TABLE [common].[ExtensionConfiguration] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Title]       NVARCHAR (255) NULL,
    [ConfigType]  NVARCHAR (255) NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_ExtensionConfiguration_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_ExtensionConfiguration] PRIMARY KEY CLUSTERED ([ID] ASC)
);

