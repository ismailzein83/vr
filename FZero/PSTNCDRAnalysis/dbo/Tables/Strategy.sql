CREATE TABLE [dbo].[Strategy] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [Description]  VARCHAR (255) NULL,
    [UserId]       INT           NOT NULL,
    [CreationDate] DATETIME      CONSTRAINT [DF_Strategy_CreationDate] DEFAULT (getdate()) NOT NULL,
    [Name]         VARCHAR (20)  NULL,
    [IsDefault]    BIT           CONSTRAINT [DF_Strategy_IsDefault] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Strategy] PRIMARY KEY CLUSTERED ([Id] ASC)
);

