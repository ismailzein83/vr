CREATE TABLE [dbo].[Changes] (
    [ID]         INT         IDENTITY (1, 1) NOT NULL,
    [ZoneID]     INT         NOT NULL,
    [CodeID]     BIGINT      NULL,
    [Changes]    SMALLINT    NOT NULL,
    [timestamp]  ROWVERSION  NULL,
    [CustomerID] VARCHAR (5) NOT NULL,
    CONSTRAINT [PK_ZoneCodeChange] PRIMARY KEY CLUSTERED ([ID] ASC)
);

