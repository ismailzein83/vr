CREATE TABLE [RA].[Period] (
    [ID]               INT             IDENTITY (1, 1) NOT NULL,
    [FromDate]         DATETIME        NULL,
    [ToDate]           DATETIME        NULL,
    [Name]             NVARCHAR (1000) NULL,
    [CreatedBy]        INT             NULL,
    [CreatedTime]      DATETIME        CONSTRAINT [DF_PeriodDeclaration_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]   INT             NULL,
    [LastModifiedTime] DATETIME        NULL,
    [timestamp]        ROWVERSION      NULL,
    [Repeat]           INT             NULL,
    CONSTRAINT [PK_PeriodDeclaration] PRIMARY KEY CLUSTERED ([ID] ASC)
);

