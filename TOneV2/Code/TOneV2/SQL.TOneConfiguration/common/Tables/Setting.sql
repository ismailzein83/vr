CREATE TABLE [common].[Setting] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (255) NOT NULL,
    [Type]      NVARCHAR (255) NOT NULL,
    [Category]  VARCHAR (255)  NOT NULL,
    [Settings]  NVARCHAR (MAX) NOT NULL,
    [Data]      NVARCHAR (MAX) NULL,
    [timestamp] ROWVERSION     NOT NULL,
    CONSTRAINT [PK_common.Setting] PRIMARY KEY CLUSTERED ([Id] ASC)
);



