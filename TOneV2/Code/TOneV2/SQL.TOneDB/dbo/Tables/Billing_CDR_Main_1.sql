CREATE TABLE [dbo].[Billing_CDR_Main] (
    [ID]                BIGINT          IDENTITY (1, 1) NOT NULL,
    [Attempt]           DATETIME        NOT NULL,
    [Alert]             DATETIME        NULL,
    [Connect]           DATETIME        NOT NULL,
    [Disconnect]        DATETIME        NOT NULL,
    [DurationInSeconds] NUMERIC (13, 4) NOT NULL,
    [CustomerID]        VARCHAR (5)     NOT NULL,
    [OurZoneID]         INT             NULL,
    [OriginatingZoneID] INT             NULL,
    [SupplierID]        VARCHAR (5)     NOT NULL,
    [SupplierZoneID]    INT             NULL,
    [CDPN]              VARCHAR (50)    NULL,
    [CGPN]              VARCHAR (50)    NULL,
    [ReleaseCode]       VARCHAR (50)    NULL,
    [ReleaseSource]     VARCHAR (10)    NULL,
    [SwitchID]          TINYINT         NOT NULL,
    [SwitchCdrID]       BIGINT          NULL,
    [Tag]               VARCHAR (50)    NULL,
    [Extra_Fields]      VARCHAR (255)   NULL,
    [Port_IN]           VARCHAR (42)    NULL,
    [Port_OUT]          VARCHAR (42)    NULL,
    [OurCode]           VARCHAR (20)    NULL,
    [SupplierCode]      VARCHAR (20)    NULL,
    [CDPNOut]           VARCHAR (40)    NULL,
    [SubscriberID]      BIGINT          NULL,
    [SIP]               VARCHAR (100)   CONSTRAINT [DF_Billing_CDR_Main_SIP] DEFAULT ('') NULL,
    CONSTRAINT [PK_Billing_CDR_Main] PRIMARY KEY CLUSTERED ([ID] ASC) ON [TOne_CDR]
);


GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Main_Supplier]
    ON [dbo].[Billing_CDR_Main]([SupplierID] ASC)
    ON [TOne_Index];


GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Main_OurZoneID]
    ON [dbo].[Billing_CDR_Main]([OurZoneID] ASC)
    ON [TOne_Index];


GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Main_Customer]
    ON [dbo].[Billing_CDR_Main]([CustomerID] ASC)
    ON [TOne_Index];


GO
CREATE NONCLUSTERED INDEX [IX_Billing_CDR_Main_Attempt]
    ON [dbo].[Billing_CDR_Main]([Attempt] ASC)
    ON [TOne_Index];

