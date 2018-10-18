CREATE TYPE [ISP].[ISPPackageType] AS TABLE (
    [ID]                        BIGINT          NULL,
    [Name]                      NVARCHAR (255)  NULL,
    [Price]                     DECIMAL (20, 8) NULL,
    [DownloadSpeed]             BIGINT          NULL,
    [CreatedTime]               DATETIME        NULL,
    [LastModifiedTime]          DATETIME        NULL,
    [UploadSpeed]               BIGINT          NULL,
    [DownloadQuota]             BIGINT          NULL,
    [UploadQuota]               BIGINT          NULL,
    [DownloadAndUploadQuota]    BIGINT          NULL,
    [DailyDownloadQuota]        BIGINT          NULL,
    [DailyUploadQuota]          BIGINT          NULL,
    [DailyFairUsagePolicyQuota] BIGINT          NULL);

