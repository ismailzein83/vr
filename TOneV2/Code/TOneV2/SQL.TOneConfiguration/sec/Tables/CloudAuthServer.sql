CREATE TABLE [sec].[CloudAuthServer] (
    [ID]                        INT            IDENTITY (1, 1) NOT NULL,
    [Settings]                  NVARCHAR (MAX) NULL,
    [ApplicationIdentification] NVARCHAR (MAX) NULL,
    [CreatedTime]               DATETIME       CONSTRAINT [DF_CloudAuthServer_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]                 ROWVERSION     NULL,
    CONSTRAINT [PK_CloudAuthServer] PRIMARY KEY CLUSTERED ([ID] ASC)
);

