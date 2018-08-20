CREATE TABLE [common].[VRComment] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [DefinitionId]     UNIQUEIDENTIFIER NULL,
    [ObjectId]         VARCHAR (50)     NULL,
    [Content]          NVARCHAR (MAX)   NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_VRComment_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_VRComment_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_VRComment] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_VRComment]
    ON [common].[VRComment]([DefinitionId] ASC, [ObjectId] ASC);

