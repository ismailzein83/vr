CREATE TABLE [SOM].[SOMRequest] (
    [ID]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [RequestTypeID]     UNIQUEIDENTIFIER NOT NULL,
    [EntityID]          VARCHAR (255)    NULL,
    [Settings]          NVARCHAR (MAX)   NULL,
    [ProcessInstanceID] BIGINT           NULL,
    [CreatedTime]       DATETIME         CONSTRAINT [DF_SOMRequest_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]         ROWVERSION       NULL,
    CONSTRAINT [PK_SOMRequest] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_SOMRequest_EntityID]
    ON [SOM].[SOMRequest]([EntityID] ASC);

