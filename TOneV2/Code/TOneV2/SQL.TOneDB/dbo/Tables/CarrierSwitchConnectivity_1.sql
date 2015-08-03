CREATE TABLE [dbo].[CarrierSwitchConnectivity] (
    [ID]                      INT            IDENTITY (1, 1) NOT NULL,
    [CarrierAccountID]        VARCHAR (10)   NOT NULL,
    [SwitchID]                TINYINT        NOT NULL,
    [Name]                    NVARCHAR (255) NULL,
    [Notes]                   NVARCHAR (MAX) NULL,
    [ConnectionType]          TINYINT        CONSTRAINT [DF_CarrierSwitchConnectivity_ConnectionType] DEFAULT ((0)) NULL,
    [NumberOfChannels_In]     INT            NULL,
    [NumberOfChannels_Out]    INT            NULL,
    [NumberOfChannels_Total]  INT            NULL,
    [BeginEffectiveDate]      SMALLDATETIME  NOT NULL,
    [EndEffectiveDate]        SMALLDATETIME  NULL,
    [UserID]                  INT            NULL,
    [Margin_Total]            REAL           NULL,
    [NumberOfChannels_Shared] INT            NULL,
    [Details]                 NVARCHAR (MAX) NULL,
    [timestamp]               ROWVERSION     NOT NULL,
    CONSTRAINT [PK_CarrierSwitchConnectivity] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_CSC_CarrierAccount]
    ON [dbo].[CarrierSwitchConnectivity]([ID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'1 for Voip and 0 for TDM', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'CarrierSwitchConnectivity', @level2type = N'COLUMN', @level2name = N'ConnectionType';

