CREATE TABLE [TOneWhS_BE].[Deal] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Settings]    NVARCHAR (MAX) NOT NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_Deal_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_Deal] PRIMARY KEY CLUSTERED ([ID] ASC)
);

