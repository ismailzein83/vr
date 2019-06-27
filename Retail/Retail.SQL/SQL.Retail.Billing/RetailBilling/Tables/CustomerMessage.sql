CREATE TABLE [RetailBilling].[CustomerMessage] (
    [ID]                    BIGINT           IDENTITY (1, 1) NOT NULL,
    [Message]               NVARCHAR (255)   NULL,
    [CustomerTypeId]        UNIQUEIDENTIFIER NULL,
    [CustomerCategoryId]    UNIQUEIDENTIFIER NULL,
    [CustomerSubCategoryId] UNIQUEIDENTIFIER NULL,
    [Type]                  INT              NULL,
    [CreatedTime]           DATETIME         NULL,
    [CreatedBy]             INT              NULL,
    [LastModifiedTime]      DATETIME         NULL,
    [LastModifiedBy]        INT              NULL,
    CONSTRAINT [PK__Customer__3214EC272B0A656D] PRIMARY KEY CLUSTERED ([ID] ASC)
);



