CREATE TYPE [NetworkRentalManager].[OrdersDefinitionType] AS TABLE (
    [ID]                     BIGINT          NULL,
    [OrderType]              INT             NULL,
    [StatusId]               INT             NULL,
    [AccountId]              BIGINT          NULL,
    [ProductId]              INT             NULL,
    [CreatedTime]            DATETIME        NULL,
    [CreatedBy]              INT             NULL,
    [LastModifiedTime]       DATETIME        NULL,
    [LastModifiedBy]         INT             NULL,
    [SubContractorId]        BIGINT          NULL,
    [ServiceID]              NVARCHAR (255)  NULL,
    [ContractTenure]         INT             NULL,
    [BillStartDate]          DATETIME        NULL,
    [BillingFrequency]       NVARCHAR (255)  NULL,
    [InstallationDate]       DATETIME        NULL,
    [MRC]                    DECIMAL (20, 4) NULL,
    [NRC]                    DECIMAL (20, 4) NULL,
    [OneTimeHardwareCharges] DECIMAL (20, 4) NULL);



