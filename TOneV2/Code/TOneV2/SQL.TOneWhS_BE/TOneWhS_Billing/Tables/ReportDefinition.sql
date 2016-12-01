CREATE TABLE [TOneWhS_Billing].[ReportDefinition] (
    [ReportDefinitionId] INT            IDENTITY (1, 1) NOT NULL,
    [ReportName]         NVARCHAR (50)  NOT NULL,
    [Content]            NVARCHAR (MAX) NULL,
    [CreatedTime]        DATETIME       CONSTRAINT [DF_ReportDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_ReportDefinition] PRIMARY KEY CLUSTERED ([ReportDefinitionId] ASC)
);



