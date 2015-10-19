CREATE TABLE [TOneWhS_BE].[CarrierProfile] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (255) NULL,
    [Settings]  NVARCHAR (MAX) NULL,
    [timestamp] ROWVERSION     NULL,
    CONSTRAINT [PK_CarrierProfile] PRIMARY KEY CLUSTERED ([ID] ASC)
);

