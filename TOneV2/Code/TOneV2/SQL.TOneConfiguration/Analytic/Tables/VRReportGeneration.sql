CREATE TABLE [Analytic].[VRReportGeneration] (
    [ID]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)  NULL,
    [Description]      NVARCHAR (1000) NULL,
    [Settings]         NVARCHAR (MAX)  NULL,
    [AccessLevel]      INT             NOT NULL,
    [CreatedTime]      DATETIME        CONSTRAINT [DF_VRReportGenerations_CreatedTime] DEFAULT (getdate()) NOT NULL,
    [LastModifiedTime] DATETIME        CONSTRAINT [DF_VRReportGenerations_LastModifiedTime] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]        INT             NOT NULL,
    [LastModifiedBy]   INT             NOT NULL,
    [timestamp]        ROWVERSION      NULL,
    CONSTRAINT [PK_VRReportGenerations] PRIMARY KEY CLUSTERED ([ID] ASC)
);

