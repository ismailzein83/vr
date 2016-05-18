CREATE TABLE [common].[EmailTemplate] (
    [ID]              INT            IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (255) NOT NULL,
    [BodyTemplate]    NVARCHAR (MAX) NOT NULL,
    [SubjectTemplate] NVARCHAR (MAX) NOT NULL,
    [Type]            NVARCHAR (255) NOT NULL,
    [timestamp]       ROWVERSION     NULL,
    CONSTRAINT [PK_Common.EmailTemplate] PRIMARY KEY CLUSTERED ([ID] ASC)
);

