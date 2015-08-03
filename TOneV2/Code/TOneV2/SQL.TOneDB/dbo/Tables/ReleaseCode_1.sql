CREATE TABLE [dbo].[ReleaseCode] (
    [Code]        INT            NOT NULL,
    [Description] NVARCHAR (MAX) NOT NULL,
    [timestamp]   ROWVERSION     NOT NULL,
    [DS_ID_auto]  INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_ReleaseCode] PRIMARY KEY CLUSTERED ([Code] ASC)
);

