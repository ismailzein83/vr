CREATE TABLE [common].[VRDevProject] (
    [ID]                  UNIQUEIDENTIFIER NOT NULL,
    [Name]                NVARCHAR (255)   NOT NULL,
    [AssemblyID]          UNIQUEIDENTIFIER NULL,
    [CreatedTime]         DATETIME         CONSTRAINT [DF_VRDevProject_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime]    DATETIME         CONSTRAINT [DF_VRDevProject_LastModifiedTime] DEFAULT (getdate()) NULL,
    [timestamp]           ROWVERSION       NULL,
    [ProjectDependencies] NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_VRDevProject] PRIMARY KEY CLUSTERED ([ID] ASC)
);





