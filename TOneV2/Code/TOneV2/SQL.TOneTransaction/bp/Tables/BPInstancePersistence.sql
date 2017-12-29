CREATE TABLE [bp].[BPInstancePersistence] (
    [ProcessInstanceID] BIGINT         NOT NULL,
    [BPState]           NVARCHAR (MAX) NULL,
    [CreatedTime]       DATETIME       CONSTRAINT [DF_BPInstancePersistence_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]         ROWVERSION     NULL,
    CONSTRAINT [PK_BPInstancePersistence] PRIMARY KEY CLUSTERED ([ProcessInstanceID] ASC)
);

