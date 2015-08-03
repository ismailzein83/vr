CREATE TABLE [dbo].[tmpZonesImportedPricelist_1cqzifa4402fiv55woqwagzl] (
    [ZoneID]             INT            NULL,
    [CodeGroup]          VARCHAR (20)   NULL,
    [Name]               NVARCHAR (255) NOT NULL,
    [SupplierID]         VARCHAR (5)    NOT NULL,
    [ServicesFlag]       SMALLINT       NULL,
    [IsMobile]           CHAR (1)       NULL,
    [IsProper]           CHAR (1)       NULL,
    [IsSold]             CHAR (1)       NULL,
    [BeginEffectiveDate] SMALLDATETIME  NOT NULL,
    [EndEffectiveDate]   SMALLDATETIME  NULL,
    [UserID]             INT            NULL
);

