CREATE TABLE [TOneWhS_SPL].[SupplierCode_Preview] (
    [ProcessInstanceID] BIGINT         NOT NULL,
    [Code]              VARCHAR (20)   NOT NULL,
    [ChangeType]        INT            NOT NULL,
    [RecentZoneName]    NVARCHAR (255) NULL,
    [ZoneName]          NVARCHAR (255) NOT NULL,
    [BED]               DATETIME       NOT NULL,
    [EED]               DATETIME       NULL,
    [IsExcluded]        BIT            NULL
);










GO
CREATE CLUSTERED INDEX [IX_SupplierCode_Preview_ProcessInstanceID]
    ON [TOneWhS_SPL].[SupplierCode_Preview]([ProcessInstanceID] ASC);

