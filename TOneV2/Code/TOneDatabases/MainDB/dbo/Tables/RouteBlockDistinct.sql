CREATE TABLE [dbo].[RouteBlockDistinct] (
    [CustomerID]          VARCHAR (5)   NULL,
    [SupplierID]          VARCHAR (5)   NULL,
    [ZoneID]              INT           NULL,
    [Code]                VARCHAR (20)  NULL,
    [BeginEffectiveDate]  SMALLDATETIME NULL,
    [EndEffectiveDate]    SMALLDATETIME NULL,
    [BlockType]           TINYINT       NULL,
    [iseffective]         VARCHAR (1)   NOT NULL,
    [UserID]              INT           NULL,
    [Reason]              VARCHAR (250) NULL,
    [RouteChangeHeaderID] INT           NULL
);

