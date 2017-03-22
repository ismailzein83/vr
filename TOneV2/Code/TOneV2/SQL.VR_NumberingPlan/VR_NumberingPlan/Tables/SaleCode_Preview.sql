CREATE TABLE [VR_NumberingPlan].[SaleCode_Preview] (
    [ProcessInstanceID] BIGINT         NOT NULL,
    [Code]              VARCHAR (20)   NOT NULL,
    [ChangeType]        INT            NOT NULL,
    [RecentZoneName]    NVARCHAR (255) NULL,
    [ZoneName]          NVARCHAR (255) NOT NULL,
    [BED]               DATETIME       NOT NULL,
    [EED]               DATETIME       NULL
);

