CREATE TABLE [sec].[CloudConnection] (
    [ID]                        INT            IDENTITY (1, 1) NOT NULL,
    [Settings]                  NVARCHAR (MAX) NULL,
    [ApplicationIdentification] NVARCHAR (MAX) NULL,
    [CreatedTime]               DATETIME       CONSTRAINT [DF_CloudHost_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]                 ROWVERSION     NULL,
    CONSTRAINT [PK_CloudHost] PRIMARY KEY CLUSTERED ([ID] ASC)
);

