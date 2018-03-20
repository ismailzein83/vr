CREATE TABLE [common].[Currency] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [Symbol]           NVARCHAR (10)  NOT NULL,
    [Name]             NVARCHAR (255) NOT NULL,
    [timestamp]        ROWVERSION     NULL,
    [SourceID]         VARCHAR (50)   NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [CreatedTime]      DATETIME       CONSTRAINT [DF_Currency_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_Currency_Symbol] UNIQUE NONCLUSTERED ([Symbol] ASC)
);







