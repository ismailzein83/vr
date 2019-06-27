CREATE TABLE [RetailBilling].[DunningRules] (
    [ID]                    BIGINT           IDENTITY (1, 1) NOT NULL,
    [CustomerId]            BIGINT           NULL,
    [CustomerCategoryId]    UNIQUEIDENTIFIER NULL,
    [CustomerSubCategoryId] UNIQUEIDENTIFIER NULL,
    [Actions]               NVARCHAR (MAX)   NULL,
    [CreatedTime]           DATETIME         NULL,
    [CreatedBy]             INT              NULL,
    [LastModifiedTime]      DATETIME         NULL,
    [LastModifiedBy]        INT              NULL,
    [CustomerTypeId]        UNIQUEIDENTIFIER NULL,
    [ExcludeFromDunning]    BIT              NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);



