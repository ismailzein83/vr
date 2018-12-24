CREATE TABLE [common].[StyleDefinition] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             VARCHAR (255)    NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_ColorDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_StyleDefinition_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_ColorDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);



