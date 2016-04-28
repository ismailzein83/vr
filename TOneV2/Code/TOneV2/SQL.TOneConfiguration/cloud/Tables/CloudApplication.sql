CREATE TABLE [cloud].[CloudApplication] (
    [ID]                        INT            IDENTITY (1, 1) NOT NULL,
    [Name]                      NVARCHAR (255) NOT NULL,
    [Settings]                  NVARCHAR (MAX) NOT NULL,
    [ApplicationIdentification] NVARCHAR (MAX) NULL,
    [CreatedTime]               DATETIME       CONSTRAINT [DF_CloudApplication_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]                 ROWVERSION     NULL,
    CONSTRAINT [PK_CloudApplication] PRIMARY KEY CLUSTERED ([ID] ASC)
);

