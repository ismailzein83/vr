CREATE TABLE [sec].[EncryptionKey] (
    [ID]            INT            IDENTITY (1, 1) NOT NULL,
    [EncryptionKey] NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_EncryptionKey] PRIMARY KEY CLUSTERED ([ID] ASC)
);

