CREATE TABLE [dbo].[SpecialRequestHistoy] (
    [SpecialRequestID]    INT           IDENTITY (1, 1) NOT NULL,
    [CustomerID]          VARCHAR (5)   NOT NULL,
    [ZoneID]              INT           NULL,
    [Code]                VARCHAR (20)  NULL,
    [SupplierID]          VARCHAR (5)   NOT NULL,
    [Priority]            TINYINT       NULL,
    [NumberOfTries]       TINYINT       NULL,
    [SpecialRequestType]  TINYINT       NOT NULL,
    [BeginEffectiveDate]  SMALLDATETIME NOT NULL,
    [EndEffectiveDate]    SMALLDATETIME NULL,
    [IsEffective]         VARCHAR (1)   NOT NULL,
    [UserID]              INT           NULL,
    [timestamp]           ROWVERSION    NULL,
    [Percentage]          TINYINT       NULL,
    [Reason]              VARCHAR (250) NULL,
    [RouteChangeHeaderID] INT           NULL
);

