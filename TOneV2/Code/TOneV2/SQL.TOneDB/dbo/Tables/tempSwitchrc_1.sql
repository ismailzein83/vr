CREATE TABLE [dbo].[tempSwitchrc] (
    [SwitchID]    TINYINT        NOT NULL,
    [ReleaseCode] VARCHAR (50)   NOT NULL,
    [IsoCode]     VARCHAR (50)   NULL,
    [Description] NVARCHAR (MAX) NULL,
    [IsDelivered] CHAR (1)       NOT NULL
);

