CREATE TABLE [common].[TemplateConfig] (
    [ID]           INT             IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (255)  NOT NULL,
    [ConfigType]   NVARCHAR (255)  NOT NULL,
    [TemplateURL]  NVARCHAR (1000) NULL,
    [BehaviorFQTN] NVARCHAR (1000) NULL,
    CONSTRAINT [PK_TemplateConfig] PRIMARY KEY CLUSTERED ([ID] ASC)
);

