CREATE TABLE [dbo].[BQR_Criterion] (
    [ID]                                    INT           IDENTITY (1, 1) NOT NULL,
    [Name]                                  VARCHAR (50)  NOT NULL,
    [CustomerID]                            VARCHAR (5)   NULL,
    [SupplierID]                            VARCHAR (5)   NULL,
    [ReleaseCode]                           VARCHAR (100) NULL,
    [MinimumSaleZoneAttempts]               INT           NOT NULL,
    [MinimumSupplierZoneAttemptsPercentage] INT           NOT NULL,
    [TimeSpan]                              VARCHAR (50)  NOT NULL,
    [IsActive]                              CHAR (1)      NOT NULL,
    [timestamp]                             ROWVERSION    NOT NULL,
    [ShouldSendEmail]                       CHAR (1)      NOT NULL,
    [ActionsNeedConfirmation]               CHAR (1)      NOT NULL,
    [Email]                                 VARCHAR (255) NULL,
    [UserID]                                INT           NULL,
    [DateTimeUpdated]                       DATETIME      NOT NULL,
    [EndEffectiveDate]                      DATETIME      NULL,
    CONSTRAINT [PK_BQR_Criterion] PRIMARY KEY CLUSTERED ([ID] ASC)
);

