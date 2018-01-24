CREATE TABLE [SOM].[SOMRequest] (
    [ID]                UNIQUEIDENTIFIER NOT NULL,
    [EntityID]          VARCHAR (255)    NULL,
    [SequenceNumber]    BIGINT           IDENTITY (1, 1) NOT NULL,
    [RequestTypeID]     UNIQUEIDENTIFIER NOT NULL,
    [Title]             NVARCHAR (1000)  NULL,
    [Settings]          NVARCHAR (MAX)   NULL,
    [ProcessInstanceID] BIGINT           NULL,
    [CreatedTime]       DATETIME         CONSTRAINT [DF_SOMRequest_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]         ROWVERSION       NULL,
    CONSTRAINT [IX_SOMRequest_ID] UNIQUE NONCLUSTERED ([ID] ASC),
    CONSTRAINT [IX_SOMRequest_SeqNb] UNIQUE NONCLUSTERED ([SequenceNumber] ASC)
);






GO
CREATE NONCLUSTERED INDEX [IX_SOMRequest_ProcessInstanceID]
    ON [SOM].[SOMRequest]([ProcessInstanceID] ASC);


GO
CREATE CLUSTERED INDEX [IX_SOMRequest_EntityIDSeqNb]
    ON [SOM].[SOMRequest]([EntityID] ASC, [SequenceNumber] DESC);

