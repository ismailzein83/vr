CREATE TABLE [Analytic].[DataAnalysisItemDefinition] (
    [ID]                       UNIQUEIDENTIFIER NOT NULL,
    [DataAnalysisDefinitionID] UNIQUEIDENTIFIER NOT NULL,
    [Name]                     NVARCHAR (255)   NULL,
    [Settings]                 NVARCHAR (MAX)   NULL,
    [CreatedTime]              DATETIME         CONSTRAINT [DF_DataAnalysisItemDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]                ROWVERSION       NULL,
    CONSTRAINT [PK_DataAnalysisItemDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);

