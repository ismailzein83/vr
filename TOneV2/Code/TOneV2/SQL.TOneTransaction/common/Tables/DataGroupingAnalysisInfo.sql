CREATE TABLE [common].[DataGroupingAnalysisInfo] (
    [DataAnalysisName]             VARCHAR (255)    NOT NULL,
    [DistributorServiceInstanceID] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_DataGroupingAnalysisInfo] PRIMARY KEY CLUSTERED ([DataAnalysisName] ASC)
);

