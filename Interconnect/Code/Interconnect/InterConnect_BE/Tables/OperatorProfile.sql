CREATE TABLE [InterConnect_BE].[OperatorProfile] (
    [ID]                           INT            IDENTITY (1, 1) NOT NULL,
    [Name]                         NVARCHAR (255) NULL,
    [Settings]                     NVARCHAR (MAX) NULL,
    [ExtendedSettingsRecordTypeID] INT            NULL,
    [ExtendedSettings]             NVARCHAR (MAX) NULL,
    [CreatedTime]                  DATETIME       CONSTRAINT [DF_OperatorProfile_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]                    ROWVERSION     NULL,
    CONSTRAINT [PK_OperatorProfile] PRIMARY KEY CLUSTERED ([ID] ASC)
);

