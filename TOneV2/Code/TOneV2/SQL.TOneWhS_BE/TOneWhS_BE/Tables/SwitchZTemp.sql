CREATE TABLE [TOneWhS_BE].[SwitchZTemp] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50)   NOT NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [timestamp]   ROWVERSION     NULL,
    [SourceID]    VARCHAR (50)   NULL,
    [CreatedTime] DATETIME       NULL
);

