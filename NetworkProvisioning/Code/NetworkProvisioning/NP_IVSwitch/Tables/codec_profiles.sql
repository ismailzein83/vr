CREATE TABLE [NP_IVSwitch].[codec_profiles] (
    [codec_profile_id] INT           IDENTITY (1, 1) NOT NULL,
    [codec_string]     VARCHAR (100) NULL,
    [description]      VARCHAR (150) NULL,
    [create_date]      DATETIME      CONSTRAINT [DF_codec_profiles_create_date] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_codec_profiles] PRIMARY KEY CLUSTERED ([codec_profile_id] ASC)
);

