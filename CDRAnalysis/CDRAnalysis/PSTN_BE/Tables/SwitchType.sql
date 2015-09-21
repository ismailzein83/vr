CREATE TABLE [PSTN_BE].[SwitchType] (
    [ID]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_SwitchType] PRIMARY KEY CLUSTERED ([ID] ASC)
);

