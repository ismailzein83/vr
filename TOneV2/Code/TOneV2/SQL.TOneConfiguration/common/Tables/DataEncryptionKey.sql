CREATE TABLE [common].[DataEncryptionKey] (
    [ID]            INT            IDENTITY (1, 1) NOT NULL,
    [EncryptionKey] NVARCHAR (255) NULL,
    CONSTRAINT [PK_DataEncryptionKey] PRIMARY KEY CLUSTERED ([ID] ASC)
);

