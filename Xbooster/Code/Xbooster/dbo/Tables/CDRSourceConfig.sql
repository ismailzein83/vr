CREATE TABLE [dbo].[CDRSourceConfig] (
    [CDRSourceConfigId]         INT           IDENTITY (1, 1) NOT NULL,
    [Name]                      VARCHAR (100) NOT NULL,
    [CDRSource]                 VARCHAR (MAX) NOT NULL,
    [SettingsTaskExecutionInfo] VARCHAR (MAX) NULL,
    [IsPartnerCDRSource]        BIT           NOT NULL,
    [UserID]                    INT           NOT NULL,
    [timestamp]                 ROWVERSION    NULL,
    CONSTRAINT [PK_CDRSourceConfig] PRIMARY KEY CLUSTERED ([CDRSourceConfigId] ASC)
);

