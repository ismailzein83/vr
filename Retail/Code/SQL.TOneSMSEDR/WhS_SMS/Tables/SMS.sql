CREATE TABLE [WhS_SMS].[SMS] (
    [ID]                     BIGINT           NOT NULL,
    [Message]                NVARCHAR (MAX)   NULL,
    [ProxiedMessage]         NVARCHAR (MAX)   NULL,
    [DSTAddressIN]           VARCHAR (255)    NULL,
    [Attempt]                INT              NULL,
    [BalancerHost]           NVARCHAR (255)   NULL,
    [ClientRequestDate]      DATETIME         NULL,
    [ClientRespTime]         DECIMAL (20, 8)  NULL,
    [Customer]               NVARCHAR (255)   NULL,
    [CustomerAmount]         DECIMAL (20, 8)  NULL,
    [CustomerConnHost]       VARCHAR (255)    NULL,
    [CustomerConnection]     VARCHAR (255)    NULL,
    [CustomerDeliveryStatus] VARCHAR (255)    NULL,
    [CustomerPrice]          DECIMAL (20, 8)  NULL,
    [Delivered]              INT              NULL,
    [DeliveryDate]           DATETIME         NULL,
    [DeliveryTime]           DECIMAL (20, 8)  NULL,
    [DSTAddressOUT]          VARCHAR (255)    NULL,
    [FinalAttempt]           INT              NULL,
    [Operator]               NVARCHAR (255)   NULL,
    [ReplyDate]              DATETIME         NULL,
    [ReplyReceived]          INT              NULL,
    [Submitted]              INT              NULL,
    [UplinkRequestDate]      DATETIME         NULL,
    [UplinkResponseDate]     DATETIME         NULL,
    [Vendor]                 VARCHAR (255)    NULL,
    [VendorAmount]           DECIMAL (20, 8)  NULL,
    [VendorConnHost]         VARCHAR (255)    NULL,
    [VendorConnPort]         VARCHAR (255)    NULL,
    [VendorConnection]       VARCHAR (255)    NULL,
    [VendorCurrency]         VARCHAR (255)    NULL,
    [VendorPrice]            DECIMAL (20, 8)  NULL,
    [VendorRespTime]         DECIMAL (20, 8)  NULL,
    [DataSourceId]           UNIQUEIDENTIFIER NULL,
    [SRCAddressIN]           VARCHAR (255)    NULL,
    CONSTRAINT [PK_SMS] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_Attempt_SMS]
    ON [WhS_SMS].[SMS]([Attempt] ASC);

