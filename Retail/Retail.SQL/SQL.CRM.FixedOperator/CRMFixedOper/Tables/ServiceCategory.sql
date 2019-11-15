CREATE TABLE [CRMFixedOper].[ServiceCategory] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [Name]                    NVARCHAR (255)   NULL,
    [CreatedTime]             DATETIME         NULL,
    [CreatedBy]               INT              NULL,
    [LastModifiedTime]        DATETIME         NULL,
    [LastModifiedBy]          INT              NULL,
    [timestamp]               ROWVERSION       NULL,
    [GroupServicesByCategory] BIT              NULL,
    [Rank]                    INT              NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

