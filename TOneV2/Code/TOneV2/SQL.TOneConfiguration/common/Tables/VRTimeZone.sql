CREATE TABLE [common].[VRTimeZone] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [SourceID]    VARCHAR (50)   NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_TimeZone_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_TimeZone] PRIMARY KEY CLUSTERED ([ID] ASC)
);



