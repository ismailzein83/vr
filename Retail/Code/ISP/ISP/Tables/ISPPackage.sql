CREATE TABLE [ISP].[ISPPackage] (
    [ID]                        BIGINT          IDENTITY (1, 1) NOT NULL,
    [Name]                      NVARCHAR (255)  NULL,
    [Price]                     DECIMAL (20, 8) NULL,
    [DownloadSpeed]             BIGINT          NULL,
    [CreatedTime]               DATETIME        NULL,
    [LastModifiedTime]          DATETIME        NULL,
    [timestamp]                 ROWVERSION      NULL,
    [UploadSpeed]               BIGINT          NULL,
    [DownloadQuota]             BIGINT          NULL,
    [UploadQuota]               BIGINT          NULL,
    [DownloadAndUploadQuota]    BIGINT          NULL,
    [DailyDownloadQuota]        BIGINT          NULL,
    [DailyUploadQuota]          BIGINT          NULL,
    [DailyFairUsagePolicyQuota] BIGINT          NULL,
    CONSTRAINT [PK_ISPPackage] PRIMARY KEY CLUSTERED ([ID] ASC)
);

