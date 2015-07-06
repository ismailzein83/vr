CREATE TABLE [dbo].[SwitchProfiles] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [Name]            VARCHAR (50)  NOT NULL,
    [FullName]        VARCHAR (250) NOT NULL,
    [AreaCode]        VARCHAR (10)  NOT NULL,
    [SwitchType]      VARCHAR (100) NOT NULL,
    [AllowAutoImport] BIT           CONSTRAINT [DF_SwitchProfiles_AllowAutoImport] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_SwitchProfileID] PRIMARY KEY CLUSTERED ([Id] ASC)
);

