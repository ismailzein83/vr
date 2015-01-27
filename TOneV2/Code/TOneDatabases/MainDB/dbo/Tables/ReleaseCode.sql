CREATE TABLE [dbo].[ReleaseCode] (
    [Code]        INT            NOT NULL,
    [Description] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_ReleaseCode] PRIMARY KEY CLUSTERED ([Code] ASC)
);

