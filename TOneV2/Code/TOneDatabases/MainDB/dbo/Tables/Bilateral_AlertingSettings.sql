CREATE TABLE [dbo].[Bilateral_AlertingSettings] (
    [SettAlertID]           INT          IDENTITY (1, 1) NOT NULL,
    [BilateralHeaderID]     INT          NOT NULL,
    [Percentage]            INT          NULL,
    [Actions]               VARCHAR (20) NULL,
    [IsGracePeroidIncluded] CHAR (1)     NOT NULL,
    [ActionTime]            INT          NOT NULL,
    [AlertLevel]            INT          NOT NULL,
    [UserID]                INT          NULL,
    [timestamp]             ROWVERSION   NOT NULL,
    [IsDeleted]             CHAR (1)     CONSTRAINT [DF_Bilateral_AlertingSettings_IsDeleted] DEFAULT ('N') NULL,
    CONSTRAINT [PK_Bilateral_AlertingSettings] PRIMARY KEY CLUSTERED ([SettAlertID] ASC)
);

