CREATE TABLE [QM_CLITester].[Profile] (
    [ID]              INT            NOT NULL,
    [SourceProfileID] VARCHAR (255)  NULL,
    [Name]            NVARCHAR (255) NOT NULL,
    [Settings]        NVARCHAR (MAX) NULL,
    [timestamp]       ROWVERSION     NULL,
    CONSTRAINT [PK_Profile] PRIMARY KEY CLUSTERED ([ID] ASC)
);

