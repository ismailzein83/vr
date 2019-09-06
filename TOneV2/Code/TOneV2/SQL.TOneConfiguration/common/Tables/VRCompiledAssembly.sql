CREATE TABLE [common].[VRCompiledAssembly] (
    [ID]              UNIQUEIDENTIFIER NOT NULL,
    [Name]            VARCHAR (900)    NULL,
    [DevProjectID]    UNIQUEIDENTIFIER NULL,
    [AssemblyContent] VARBINARY (MAX)  NULL,
    [CompiledTime]    DATETIME         NULL,
    [CreatedTime]     DATETIME         NULL,
    CONSTRAINT [PK__VRCompil__3214EC272F7AE026] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_VRCompiledAssembly_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);




GO


