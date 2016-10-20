CREATE TABLE [bp].[BPDefinition] (
    [Name]        VARCHAR (255)    NOT NULL,
    [Title]       NVARCHAR (255)   NOT NULL,
    [FQTN]        VARCHAR (1000)   NOT NULL,
    [Config]      NVARCHAR (MAX)   NOT NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_BPDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL,
    [OldID]       INT              NULL,
    [ID]          UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    CONSTRAINT [pk_BPDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);



