CREATE TABLE [Mobile_EDR].[MobileCDRFileMetadata] (
    [ID]                       BIGINT           IDENTITY (1, 1) NOT NULL,
    [RuntimeNodeID]            UNIQUEIDENTIFIER NULL,
    [ParentFolderRelativePath] NVARCHAR (1000)  NULL,
    [FileNames]                NVARCHAR (MAX)   NULL,
    [FromTime]                 DATETIME         NULL,
    [ToTime]                   DATETIME         NULL,
    [NbOfRecords]              BIGINT           NULL,
    [MinTime]                  DATETIME         NULL,
    [MaxTime]                  DATETIME         NULL,
    [MinID]                    BIGINT           NULL,
    [MaxID]                    BIGINT           NULL,
    [IsReady]                  BIT              NOT NULL,
    [CreatedTime]              DATETIME         CONSTRAINT [DF_MobileCDRFileMetadata_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime]         DATETIME         NULL,
    [timestamp]                ROWVERSION       NULL,
    CONSTRAINT [PK_MobileCDRFileMetadata] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_MobileCDRFileMetadata_TimeAndIDBorders]
    ON [Mobile_EDR].[MobileCDRFileMetadata]([FromTime] ASC, [ToTime] ASC, [MinID] ASC, [MaxID] ASC);

