CREATE TABLE [TOneWhS_BE].[CP_SaleZone_New] (
    [ID]                  INT            NOT NULL,
    [ProcessInstanceID]   BIGINT         NOT NULL,
    [CountryID]           INT            NOT NULL,
    [Name]                NVARCHAR (255) NOT NULL,
    [SellingNumberPlanID] INT            NOT NULL,
    [BED]                 DATETIME       NOT NULL,
    [EED]                 DATETIME       NULL
);




GO
CREATE CLUSTERED INDEX [IX_CP_SaleZone_New_ProcessInstanceID]
    ON [TOneWhS_BE].[CP_SaleZone_New]([ProcessInstanceID] ASC);

