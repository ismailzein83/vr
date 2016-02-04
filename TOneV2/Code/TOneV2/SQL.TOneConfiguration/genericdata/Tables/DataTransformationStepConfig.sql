CREATE TABLE [genericdata].[DataTransformationStepConfig] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (255)  NOT NULL,
    [Details]     NVARCHAR (MAX) NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_TransformationStepDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_TransformationStepDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_TransformationStepDefinition_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

