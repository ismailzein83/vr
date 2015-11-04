CREATE TABLE [TOneWhS_CDR].[CDRFailed] (
    [ID]                INT             NULL,
    [CustomerID]        INT             NULL,
    [SupplierID]        INT             NULL,
    [Attempt]           DATETIME        NULL,
    [DurationInSeconds] NUMERIC (13, 4) NULL,
    [Alert]             DATETIME        NULL,
    [Connect]           DATETIME        NULL,
    [Disconnect]        DATETIME        NULL,
    [PortOut]           NVARCHAR (50)   NULL,
    [PortIn]            NVARCHAR (50)   NULL
);

