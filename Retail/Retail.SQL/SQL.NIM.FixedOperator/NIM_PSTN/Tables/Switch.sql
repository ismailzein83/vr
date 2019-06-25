CREATE TABLE [NIM_PSTN].[Switch] (
    [Id]                    BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]                  NVARCHAR (255)   NULL,
    [Vendor]                BIGINT           NULL,
    [Type]                  BIGINT           NULL,
    [Site]                  BIGINT           NULL,
    [OMC]                   BIGINT           NULL,
    [Services]              BIGINT           NULL,
    [ParentSwitch]          BIGINT           NULL,
    [TotalCapacity]         INT              NULL,
    [CurrentUtilization]    INT              NULL,
    [FaultyDevices]         INT              NULL,
    [UtilizationPercentage] INT              NULL,
    [Threshold]             INT              NULL,
    [Status]                UNIQUEIDENTIFIER NULL,
    [Region]                INT              NULL,
    [City]                  INT              NULL,
    [Town]                  INT              NULL,
    [CreatedBy]             INT              NULL,
    [CreatedTime]           DATETIME         NULL,
    [LastModifiedBy]        INT              NULL,
    [LastModifiedTime]      DATETIME         NULL,
    [timestamp]             ROWVERSION       NULL,
    CONSTRAINT [PK__Switch__3214EC070CBAE877] PRIMARY KEY CLUSTERED ([Id] ASC)
);





