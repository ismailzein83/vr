CREATE TABLE [dbo].[Bilateral_Details] (
    [ID]          INT             IDENTITY (1, 1) NOT NULL,
    [SupplierID]  VARCHAR (5)     NOT NULL,
    [BilateralID] INT             NOT NULL,
    [ZoneID]      INT             NOT NULL,
    [BeginDate]   DATETIME        NOT NULL,
    [EndDate]     DATETIME        NOT NULL,
    [ASR]         DECIMAL (18, 8) NULL,
    [ACD]         DECIMAL (18, 8) NULL,
    [Rate]        DECIMAL (18, 8) NOT NULL,
    [Volume]      INT             NOT NULL,
    [Amount]      DECIMAL (18, 8) NULL,
    [Description] NVARCHAR (50)   NULL,
    [UserID]      INT             NULL,
    [NER]         DECIMAL (18, 8) NULL,
    CONSTRAINT [PK_Bilateral_Details] PRIMARY KEY CLUSTERED ([ID] ASC)
);

