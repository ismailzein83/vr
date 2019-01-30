CREATE TYPE [integration].[DataSourceImportedBatchExecutionStatusType] AS TABLE (
    [ID]              BIGINT NOT NULL,
    [ExecutionStatus] INT    NOT NULL);

