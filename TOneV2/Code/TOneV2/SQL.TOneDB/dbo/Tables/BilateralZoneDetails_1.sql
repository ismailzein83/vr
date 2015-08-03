CREATE TABLE [dbo].[BilateralZoneDetails] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [ZoneID]            INT            NULL,
    [AgreementDetailID] INT            NULL,
    [ZoneConfig]        NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Bilateral_ZoneDetails] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_AgreementDetailID]
    ON [dbo].[BilateralZoneDetails]([AgreementDetailID] ASC);

