CREATE TABLE [genericdata].[ExpressionBuilderConfig] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (255)  NULL,
    [Details]          NVARCHAR (MAX) NULL,
    [CreatedTime]      DATETIME       CONSTRAINT [DF_ExpressionBuilderConfig_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION     NULL,
    [LastModifiedTime] DATETIME       CONSTRAINT [DF_ExpressionBuilderConfig_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_ExpressionBuilderConfig] PRIMARY KEY CLUSTERED ([ID] ASC)
);



