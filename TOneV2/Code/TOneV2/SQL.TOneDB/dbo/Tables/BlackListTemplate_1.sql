CREATE TABLE [dbo].[BlackListTemplate] (
    [TemplateID]    INT           IDENTITY (1, 1) NOT NULL,
    [Name]          VARCHAR (MAX) NOT NULL,
    [Priority]      TINYINT       NOT NULL,
    [MinRepeated]   INT           NULL,
    [MaxRepeated]   INT           NULL,
    [Number]        VARCHAR (255) NULL,
    [MinACD]        FLOAT (53)    NULL,
    [MaxACD]        FLOAT (53)    NULL,
    [MinASR]        FLOAT (53)    NULL,
    [MaxASR]        FLOAT (53)    NULL,
    [ZoneID]        INT           NULL,
    [MinDuration]   FLOAT (53)    NULL,
    [MaxDuration]   FLOAT (53)    NULL,
    [PreferedRoute] VARCHAR (50)  NULL,
    [UserID]        INT           NULL,
    CONSTRAINT [PK_BlackListTemplate] PRIMARY KEY CLUSTERED ([TemplateID] ASC)
);

