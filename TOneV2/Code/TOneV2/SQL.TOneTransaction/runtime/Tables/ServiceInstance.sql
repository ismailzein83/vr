CREATE TABLE [runtime].[ServiceInstance] (
    [ServiceInstanceID]   UNIQUEIDENTIFIER NOT NULL,
    [ServiceType]         UNIQUEIDENTIFIER NOT NULL,
    [ProcessID]           INT              NOT NULL,
    [ServiceInstanceInfo] NVARCHAR (MAX)   NOT NULL,
    [timestamp]           ROWVERSION       NULL,
    [CreatedTime]         DATETIME         CONSTRAINT [DF_ServiceInstance_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_ServiceInstance] PRIMARY KEY CLUSTERED ([ServiceInstanceID] ASC)
);

