CREATE TABLE [dbo].[SystemMessage] (
    [MessageID]   VARCHAR (100) NOT NULL,
    [Description] VARCHAR (MAX) NOT NULL,
    [Message]     VARCHAR (MAX) NULL,
    [Updated]     DATETIME      CONSTRAINT [DF_SystemMessage_Updated] DEFAULT (getdate()) NOT NULL,
    [timestamp]   ROWVERSION    NOT NULL,
    [DS_ID_auto]  INT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_SystemMessage] PRIMARY KEY CLUSTERED ([MessageID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_SystemMessage]
    ON [dbo].[SystemMessage]([Updated] DESC);

