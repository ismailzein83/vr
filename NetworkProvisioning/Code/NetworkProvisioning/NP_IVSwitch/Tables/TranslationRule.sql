CREATE TABLE [NP_IVSwitch].[TranslationRule] (
    [ID]           INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (255) NULL,
    [DNIS_Pattern] VARCHAR (255)  NULL,
    [CLI_Pattern]  VARCHAR (255)  NULL,
    [CreatedTime]  DATETIME       CONSTRAINT [DF_TranslationRule_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]    ROWVERSION     NULL,
    CONSTRAINT [PK_TranslationRule] PRIMARY KEY CLUSTERED ([ID] ASC)
);

