CREATE TABLE [Ringo].[AgentNumberRequest] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [AgentId]     BIGINT         NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [Status]      TINYINT        NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_AgentNumberRequest_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_AgentNumberRequest] PRIMARY KEY CLUSTERED ([Id] ASC)
);

