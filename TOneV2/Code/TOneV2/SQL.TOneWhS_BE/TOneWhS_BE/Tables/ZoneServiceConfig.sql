CREATE TABLE [TOneWhS_BE].[ZoneServiceConfig] (
    [ServiceFlag] SMALLINT       NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_ZoneServiceConfig_1] PRIMARY KEY CLUSTERED ([ServiceFlag] ASC)
);



