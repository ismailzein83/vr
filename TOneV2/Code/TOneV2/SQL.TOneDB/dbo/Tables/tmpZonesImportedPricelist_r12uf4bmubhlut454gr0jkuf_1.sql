CREATE TABLE [dbo].[tmpZonesImportedPricelist_r12uf4bmubhlut454gr0jkuf] (
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

