CREATE TABLE [dbo].[Deal] (
    [DealID]             INT           IDENTITY (1, 1) NOT NULL,
    [SupplierID]         VARCHAR (5)   NOT NULL,
    [CustomerID]         VARCHAR (5)   NOT NULL,
    [BeginEffectiveDate] SMALLDATETIME NULL,
    [EndEffectiveDate]   SMALLDATETIME NULL,
    [Description]        NVARCHAR (50) NULL,
    [IsActive]           CHAR (1)      NULL,
    [IsPercentage]       CHAR (1)      CONSTRAINT [DF_Deal_IsPercentage] DEFAULT ('N') NULL,
    [CreatedDate]        DATETIME      CONSTRAINT [DF_Deal_CreatedDate] DEFAULT (getdate()) NULL,
    [timestamp]          ROWVERSION    NOT NULL,
    CONSTRAINT [PK_Deal] PRIMARY KEY CLUSTERED ([DealID] ASC)
);

