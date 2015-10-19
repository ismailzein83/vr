CREATE TABLE [PSTN_BE].[SwitchBrand] (
    [ID]        INT          IDENTITY (1, 1) NOT NULL,
    [Name]      VARCHAR (50) NOT NULL,
    [timestamp] ROWVERSION   NULL,
    CONSTRAINT [PK_SwitchType] PRIMARY KEY CLUSTERED ([ID] ASC)
);



