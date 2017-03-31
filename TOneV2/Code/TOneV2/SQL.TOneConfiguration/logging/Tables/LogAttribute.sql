CREATE TABLE [logging].[LogAttribute] (
    [ID]            INT           IDENTITY (1, 1) NOT NULL,
    [AttributeType] INT           NOT NULL,
    [Description]   VARCHAR (850) NOT NULL,
    [timestamp]     ROWVERSION    NULL,
    CONSTRAINT [PK_LogAttribute] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_LogAttribute_TypeDescription] UNIQUE NONCLUSTERED ([AttributeType] ASC, [Description] ASC)
);

