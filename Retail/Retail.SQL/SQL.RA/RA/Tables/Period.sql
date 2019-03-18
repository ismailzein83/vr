CREATE TABLE [RA].[Period] (
    [ID]               INT             NOT NULL,
    [FromDate]         DATETIME        NULL,
    [ToDate]           DATETIME        NULL,
    [Name]             NVARCHAR (1000) NULL,
    [CreatedBy]        INT             NULL,
    [CreatedTime]      DATETIME        NULL,
    [LastModifiedBy]   INT             NULL,
    [LastModifiedTime] DATETIME        NULL,
    [Repeat]           INT             NULL,
    [timestamp]        ROWVERSION      NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

