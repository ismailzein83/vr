CREATE TABLE [dbo].[FlaggedService] (
    [FlaggedServiceID] SMALLINT        NOT NULL,
    [Symbol]           VARCHAR (3)     NULL,
    [Name]             NVARCHAR (100)  NOT NULL,
    [Description]      NVARCHAR (1024) NULL,
    [UserID]           INT             NULL,
    [timestamp]        ROWVERSION      NULL,
    [DS_ID_auto]       INT             IDENTITY (1, 1) NOT NULL,
    [ServiceColor]     VARCHAR (10)    NULL,
    CONSTRAINT [PK_FlaggedService] PRIMARY KEY CLUSTERED ([FlaggedServiceID] ASC)
);

