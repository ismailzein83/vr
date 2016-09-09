CREATE TABLE [common].[TimeZone] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [timestamp]   ROWVERSION     NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_TimeZone_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_TimeZone] PRIMARY KEY CLUSTERED ([ID] ASC)
);

