CREATE TABLE [bp].[BPDefinition] (
    [ID]          INT            NOT NULL,
    [Title]       VARCHAR (255)  NOT NULL,
    [FQTN]        VARCHAR (1000) NOT NULL,
    [Config]      NVARCHAR (MAX) NOT NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_BPDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_BPDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);

