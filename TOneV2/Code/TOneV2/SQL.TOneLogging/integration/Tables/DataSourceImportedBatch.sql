CREATE TABLE [integration].[DataSourceImportedBatch] (
    [ID]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [DataSourceId]     INT             NOT NULL,
    [BatchDescription] NVARCHAR (1000) NULL,
    [BatchSize]        DECIMAL (18, 5) NULL,
    [RecordsCount]     INT             NOT NULL,
    [MappingResult]    INT             NOT NULL,
    [MapperMessage]    NVARCHAR (MAX)  NULL,
    [QueueItemIds]     VARCHAR (255)   NOT NULL,
    [LogEntryTime]     DATETIME        NOT NULL,
    CONSTRAINT [PK_DataSourceImportedBatch] PRIMARY KEY CLUSTERED ([ID] ASC)
);







