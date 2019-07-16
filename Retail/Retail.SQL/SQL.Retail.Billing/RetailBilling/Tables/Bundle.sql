CREATE TABLE [RetailBilling].[Bundle] (
    [ID]               UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NULL,
    [Size]             DECIMAL (20, 8)  NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL
);



