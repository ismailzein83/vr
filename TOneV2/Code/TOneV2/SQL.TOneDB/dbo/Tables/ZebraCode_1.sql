CREATE TABLE [dbo].[ZebraCode] (
    [ID]          BIGINT       IDENTITY (1, 1) NOT NULL,
    [Code]        VARCHAR (20) NULL,
    [ZoneID]      INT          NULL,
    [ZoneName]    VARCHAR (50) NULL,
    [CodeGroupID] BIGINT       NULL,
    [IsCoderoup]  BIT          NULL,
    CONSTRAINT [PK_ZebraCode] PRIMARY KEY CLUSTERED ([ID] ASC)
);

