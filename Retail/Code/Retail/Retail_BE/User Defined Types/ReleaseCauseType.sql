CREATE TYPE [Retail_BE].[ReleaseCauseType] AS TABLE (
    [ID]               INT            NULL,
    [ReleaseCode]      NVARCHAR (255) NULL,
    [ReleaseCodeName]  NVARCHAR (255) NULL,
    [IsDelivered]      BIT            NULL,
    [Description]      NVARCHAR (MAX) NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL);

