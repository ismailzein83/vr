CREATE TABLE [TOneWhS_SPL].[SupplierOtherRate_Preview] (
    [ProcessInstanceID] BIGINT          NOT NULL,
    [ZoneName]          NVARCHAR (255)  NOT NULL,
    [SystemRate]        DECIMAL (20, 8) NULL,
    [SystemRateBED]     DATETIME        NULL,
    [SystemRateEED]     DATETIME        NULL,
    [ImportedRate]      DECIMAL (20, 8) NULL,
    [ImportedRateBED]   DATETIME        NULL,
    [RateTypeID]        INT             NOT NULL,
    [RateChangeType]    INT             NOT NULL
);

