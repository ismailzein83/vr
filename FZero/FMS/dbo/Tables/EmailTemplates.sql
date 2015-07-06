CREATE TABLE [dbo].[EmailTemplates] (
    [ID]             INT            NOT NULL,
    [Name]           NVARCHAR (50)  NOT NULL,
    [MessageBody]    NVARCHAR (MAX) NOT NULL,
    [Subject]        NVARCHAR (500) NOT NULL,
    [IsActive]       BIT            NOT NULL,
    [LastUpdateDate] DATETIME2 (0)  NOT NULL,
    [LastUpdatedBy]  INT            NOT NULL,
    [AppPortal]      INT            NULL,
    CONSTRAINT [PK_EmailTemplates] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_EmailTemplates_Users] FOREIGN KEY ([LastUpdatedBy]) REFERENCES [dbo].[Users] ([ID])
);

