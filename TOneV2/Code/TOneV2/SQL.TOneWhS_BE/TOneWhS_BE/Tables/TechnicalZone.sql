CREATE TABLE [TOneWhS_BE].[TechnicalZone] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [ZoneName]         NVARCHAR (255) NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK__Technica__3214EC274A064FDE] PRIMARY KEY CLUSTERED ([ID] ASC)
);

