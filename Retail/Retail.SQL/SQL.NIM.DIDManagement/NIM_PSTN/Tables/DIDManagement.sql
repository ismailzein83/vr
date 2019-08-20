CREATE TABLE [NIM_PSTN].[DIDManagement] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [AssignedTo]       NVARCHAR (255)   NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [Status]           UNIQUEIDENTIFIER NULL,
    [DID]              NVARCHAR (255)   NULL,
    [Category]         UNIQUEIDENTIFIER NULL,
    [Classification]   UNIQUEIDENTIFIER NULL,
    [AvailableFrom]    DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

