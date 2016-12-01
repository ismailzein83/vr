CREATE TABLE [TOneWhS_BE].[ZoneServiceConfig] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Symbol]      NVARCHAR (50)  NOT NULL,
    [Settings]    NVARCHAR (MAX) NOT NULL,
    [SourceID]    VARCHAR (50)   NULL,
    [timestamp]   ROWVERSION     NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_ZoneServiceConfig_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_ZoneServiceConfig] PRIMARY KEY CLUSTERED ([ID] ASC)
);










GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ZoneServiceConfig]
    ON [TOneWhS_BE].[ZoneServiceConfig]([Symbol] ASC);

