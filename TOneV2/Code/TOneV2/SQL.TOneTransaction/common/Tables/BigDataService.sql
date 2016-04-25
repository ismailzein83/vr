CREATE TABLE [common].[BigDataService] (
    [ID]                      BIGINT         IDENTITY (1, 1) NOT NULL,
    [ServiceURL]              VARCHAR (1000) NOT NULL,
    [RuntimeProcessID]        INT            NOT NULL,
    [TotalCachedRecordsCount] BIGINT         NULL,
    [CachedObjectIds]         VARCHAR (MAX)  NULL,
    [CreatedTime]             DATETIME       CONSTRAINT [DF_BigDataService_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]               ROWVERSION     NULL,
    CONSTRAINT [PK_BigDataService] PRIMARY KEY CLUSTERED ([ID] ASC)
);

