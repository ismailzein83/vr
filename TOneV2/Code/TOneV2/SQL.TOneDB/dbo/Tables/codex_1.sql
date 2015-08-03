CREATE TABLE [dbo].[codex] (
    [ID]                 BIGINT        NOT NULL,
    [Code]               VARCHAR (20)  NOT NULL,
    [ZoneID]             INT           NOT NULL,
    [BeginEffectiveDate] SMALLDATETIME NOT NULL,
    [EndEffectiveDate]   SMALLDATETIME NULL,
    [IsEffective]        VARCHAR (1)   NOT NULL,
    [UserID]             INT           NULL,
    [timestamp]          ROWVERSION    NOT NULL
);

