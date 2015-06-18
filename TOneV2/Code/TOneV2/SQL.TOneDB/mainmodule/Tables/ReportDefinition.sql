CREATE TABLE [mainmodule].[ReportDefinition] (
    [ReportDefinitionId] INT            IDENTITY (1, 1) NOT NULL,
    [ReportName]         NVARCHAR (50)  NOT NULL,
    [Content]            NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ReportDefinition] PRIMARY KEY CLUSTERED ([ReportDefinitionId] ASC)
);

