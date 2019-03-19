CREATE TABLE [RA].[Period] (
    [ID]               INT             IDENTITY (1, 1) NOT NULL,
    [FromDate]         DATETIME        NULL,
    [ToDate]           DATETIME        NULL,
    [Name]             NVARCHAR (1000) NULL,
    [CreatedBy]        INT             NULL,
    [CreatedTime]      DATETIME        NULL,
    [LastModifiedBy]   INT             NULL,
    [LastModifiedTime] DATETIME        NULL,
    [Repeat]           INT             NULL,
    [timestamp]        ROWVERSION      NULL,
    CONSTRAINT [PK__Period__3214EC270E6E26BF] PRIMARY KEY CLUSTERED ([ID] ASC)
);



