CREATE TABLE [Mediation_Generic].[MediationDefinition] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        VARCHAR (255)    NOT NULL,
    [Details]     NVARCHAR (MAX)   NOT NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_MediationSettingDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL
);



