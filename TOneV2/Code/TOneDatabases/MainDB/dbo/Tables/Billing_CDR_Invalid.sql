CREATE TABLE [dbo].[Billing_CDR_Invalid] (
    [ID]                BIGINT          IDENTITY (1, 1) NOT NULL,
    [Attempt]           DATETIME        NULL,
    [Alert]             DATETIME        NULL,
    [Connect]           DATETIME        NULL,
    [Disconnect]        DATETIME        NULL,
    [DurationInSeconds] NUMERIC (13, 4) NULL,
    [CustomerID]        VARCHAR (5)     NULL,
    [OurZoneID]         INT             NULL,
    [SupplierID]        VARCHAR (5)     NULL,
    [SupplierZoneID]    INT             NULL,
    [CDPN]              VARCHAR (50)    NULL,
    [CGPN]              VARCHAR (50)    NULL,
    [ReleaseCode]       VARCHAR (50)    NULL,
    [ReleaseSource]     VARCHAR (10)    NULL,
    [SwitchID]          TINYINT         NULL,
    [SwitchCdrID]       BIGINT          NULL,
    [Tag]               VARCHAR (50)    NULL,
    [OriginatingZoneID] INT             NULL,
    [Extra_Fields]      VARCHAR (255)   NULL,
    [IsRerouted]        CHAR (1)        CONSTRAINT [DF_Billing_CDR_Invalid_Is_Rerouted] DEFAULT ('N') NULL,
    [Port_IN]           VARCHAR (42)    NULL,
    [Port_OUT]          VARCHAR (42)    NULL,
    [OurCode]           VARCHAR (20)    NULL,
    [SupplierCode]      VARCHAR (20)    NULL,
    [CDPNOut]           VARCHAR (50)    NULL,
    [SubscriberID]      BIGINT          NULL,
    [SIP]               VARCHAR (100)   NULL,
    [DaysSince2000]     INT             NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Invalid_Supplier]
    ON [dbo].[Billing_CDR_Invalid]([SupplierID] ASC)
    ON [TOne_Index];


GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Invalid_OurZoneID]
    ON [dbo].[Billing_CDR_Invalid]([OurZoneID] ASC)
    ON [TOne_Index];


GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Invalid_Customer]
    ON [dbo].[Billing_CDR_Invalid]([CustomerID] ASC)
    ON [TOne_Index];


GO
CREATE CLUSTERED INDEX [IX_Billing_CDR_Invalid_Attempt]
    ON [dbo].[Billing_CDR_Invalid]([Attempt] DESC)
    ON [TOne_CDR];

