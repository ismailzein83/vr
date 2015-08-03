CREATE TABLE [dbo].[PriceListChangeLog] (
    [LogID]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [PriceListID] INT            NOT NULL,
    [Date]        DATETIME       NOT NULL,
    [ObjectID]    BIGINT         NOT NULL,
    [ObjectType]  CHAR (1)       NOT NULL,
    [ChangeType]  INT            NOT NULL,
    [Description] NVARCHAR (200) NOT NULL,
    [BED]         DATETIME       NULL,
    [EED]         DATETIME       NULL,
    [timestamp]   ROWVERSION     NOT NULL,
    CONSTRAINT [PK_PriceListChangeLog] PRIMARY KEY CLUSTERED ([LogID] ASC)
);

