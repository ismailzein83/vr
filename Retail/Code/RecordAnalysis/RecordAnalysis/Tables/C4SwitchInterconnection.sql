CREATE TABLE [RecordAnalysis].[C4SwitchInterconnection] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [SwitchId]          INT            NULL,
    [InterconnectionId] INT            NULL,
    [Settings]          NVARCHAR (MAX) NULL,
    [CreatedBy]         INT            NULL,
    [CreatedTime]       DATETIME       NULL,
    [LastModifiedBy]    INT            NULL,
    [LastModifiedTime]  DATETIME       NULL,
    [timestamp]         ROWVERSION     NULL,
    CONSTRAINT [PK_SwitchInterConnection] PRIMARY KEY CLUSTERED ([Id] ASC)
);

