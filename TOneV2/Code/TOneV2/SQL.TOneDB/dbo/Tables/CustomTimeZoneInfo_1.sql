CREATE TABLE [dbo].[CustomTimeZoneInfo] (
    [ID]            SMALLINT      IDENTITY (1, 1) NOT NULL,
    [BaseUtcOffset] INT           NOT NULL,
    [DisplayName]   VARCHAR (250) NOT NULL,
    [UserID]        INT           NULL,
    [Timestamp]     ROWVERSION    NOT NULL,
    CONSTRAINT [PK_CustomTimeZone] PRIMARY KEY CLUSTERED ([ID] ASC)
);

