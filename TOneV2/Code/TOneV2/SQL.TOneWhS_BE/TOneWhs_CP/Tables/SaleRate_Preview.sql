CREATE TABLE [TOneWhs_CP].[SaleRate_Preview] (
    [ProcessInstanceID] BIGINT          NOT NULL,
    [OwnerType]         TINYINT         NOT NULL,
    [OwnerID]           INT             NOT NULL,
    [Rate]              DECIMAL (20, 8) NOT NULL,
    [BED]               DATETIME        NOT NULL,
    [EED]               DATETIME        NULL,
    [ZoneName]          NVARCHAR (255)  NOT NULL,
    [CurrencyID]        INT             NULL
);








GO
CREATE CLUSTERED INDEX [IX_SaleRate_Preview_ProcessInstanceID]
    ON [TOneWhs_CP].[SaleRate_Preview]([ProcessInstanceID] ASC);

