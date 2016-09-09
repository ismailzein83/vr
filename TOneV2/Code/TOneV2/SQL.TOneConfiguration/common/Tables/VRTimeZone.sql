CREATE TABLE [common].[VRTimeZone] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [timestamp]   ROWVERSION     NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_TimeZone_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_TimeZone] PRIMARY KEY CLUSTERED ([ID] ASC)
);

