CREATE TYPE [common].[VRCommentType] AS TABLE (
    [ID]               BIGINT           NULL,
    [DefinitionId]     UNIQUEIDENTIFIER NULL,
    [ObjectId]         VARCHAR (50)     NULL,
    [Content]          NVARCHAR (MAX)   NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL);

