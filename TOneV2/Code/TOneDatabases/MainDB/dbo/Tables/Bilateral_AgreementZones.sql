CREATE TABLE [dbo].[Bilateral_AgreementZones] (
    [ID]          INT           IDENTITY (1, 1) NOT NULL,
    [Description] NVARCHAR (50) NULL,
    [SupplierID]  VARCHAR (5)   NOT NULL,
    [AgreementID] INT           NOT NULL,
    [ZoneID]      INT           NOT NULL,
    [ASR]         FLOAT (53)    NULL,
    [ACD]         FLOAT (53)    NULL,
    [Rate]        FLOAT (53)    NOT NULL,
    [Volume]      INT           NOT NULL,
    [Amount]      FLOAT (53)    NULL,
    [NER]         FLOAT (53)    NULL,
    [UserID]      INT           NULL,
    [MaxcostRate] FLOAT (53)    NULL,
    [MinSaleRate] FLOAT (53)    NULL,
    CONSTRAINT [PK_Bilateral_AgreementZones] PRIMARY KEY CLUSTERED ([ID] ASC)
);

