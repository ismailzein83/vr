CREATE TABLE [dbo].[RouteBlockHistoy] (
    [RouteBlockID]        INT           IDENTITY (1, 1) NOT NULL,
    [CustomerID]          VARCHAR (5)   NULL,
    [SupplierID]          VARCHAR (5)   NULL,
    [ZoneID]              INT           NULL,
    [Code]                VARCHAR (20)  NULL,
    [UserID]              INT           NULL,
    [UpdateDate]          SMALLDATETIME NULL,
    [BeginEffectiveDate]  SMALLDATETIME NULL,
    [EndEffectiveDate]    SMALLDATETIME NULL,
    [BlockType]           TINYINT       NULL,
    [IsEffective]         VARCHAR (1)   NOT NULL,
    [timestamp]           ROWVERSION    NULL,
    [Reason]              VARCHAR (250) NULL,
    [RouteChangeHeaderID] INT           NULL
);

