CREATE TABLE [dbo].[SwitchProfiles] (
    [Id]              INT          IDENTITY (1, 1) NOT NULL,
    [Name]            VARCHAR (50) NOT NULL,
    [AllowAutoImport] BIT          CONSTRAINT [DF_SwitchProfiles_AllowAutoImport] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_SwitchProfileID] PRIMARY KEY CLUSTERED ([Id] ASC)
);

