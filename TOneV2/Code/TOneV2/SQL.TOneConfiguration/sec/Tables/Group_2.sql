CREATE TABLE [sec].[Group] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Description] NTEXT          NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_Role_2] PRIMARY KEY CLUSTERED ([ID] ASC)
);



