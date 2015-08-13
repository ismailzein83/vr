CREATE TABLE [sec].[User] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Password]    NVARCHAR (255) NOT NULL,
    [Email]       NVARCHAR (255) NULL,
    [Status]      INT            NULL,
    [LastLogin]   SMALLDATETIME  NULL,
    [Description] NTEXT          NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([ID] ASC)
);



