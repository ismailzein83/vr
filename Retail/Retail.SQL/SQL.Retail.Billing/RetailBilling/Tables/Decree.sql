CREATE TABLE [RetailBilling].[Decree] (
    [ID]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [DecreeNumber]     NVARCHAR (255) NULL,
    [Name]             NVARCHAR (255) NULL,
    [Date]             DATETIME       NULL,
    [Attachment]       NVARCHAR (MAX) NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

