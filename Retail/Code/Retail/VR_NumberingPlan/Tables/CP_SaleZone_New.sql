CREATE TABLE [VR_NumberingPlan].[CP_SaleZone_New] (
    [ID]                  INT            NOT NULL,
    [ProcessInstanceID]   BIGINT         NOT NULL,
    [CountryID]           INT            NOT NULL,
    [Name]                NVARCHAR (255) NOT NULL,
    [SellingNumberPlanID] INT            NOT NULL,
    [BED]                 DATETIME       NOT NULL,
    [EED]                 DATETIME       NULL
);

