(function (appControllers) {

    'use strict';

    ReceivedSupplierPricelistService.$inject = ['WhS_SupPL_ReceivedPricelistStatusEnum', 'LabelColorsEnum'];

    function ReceivedSupplierPricelistService(WhS_SupPL_ReceivedPricelistStatusEnum, LabelColorsEnum) {

        return ({
            getStatusColor: getStatusColor,
        });

        function getStatusColor(status) {
            if (status === WhS_SupPL_ReceivedPricelistStatusEnum.Received.value) return LabelColorsEnum.New.color;
            if (status === WhS_SupPL_ReceivedPricelistStatusEnum.Processing.value) return LabelColorsEnum.Processing.color;
            if (status === WhS_SupPL_ReceivedPricelistStatusEnum.Succeeded.value) return LabelColorsEnum.Success.color;
            if (status === WhS_SupPL_ReceivedPricelistStatusEnum.CompletedWithNoChanges.value) return LabelColorsEnum.Success.color;
            if (status === WhS_SupPL_ReceivedPricelistStatusEnum.FailedDueToBusinessRuleError.value) return LabelColorsEnum.Error.color;
            if (status === WhS_SupPL_ReceivedPricelistStatusEnum.FailedDueToProcessingError.value) return LabelColorsEnum.Error.color;
            if (status === WhS_SupPL_ReceivedPricelistStatusEnum.FailedDueToConfigurationError.value) return LabelColorsEnum.Error.color;
            if (status === WhS_SupPL_ReceivedPricelistStatusEnum.FailedDueToReceivedMailError.value) return LabelColorsEnum.Error.color;
            if (status === WhS_SupPL_ReceivedPricelistStatusEnum.ImportedManually.value) return LabelColorsEnum.Success.color;
            if (status === WhS_SupPL_ReceivedPricelistStatusEnum.Rejected.value) return LabelColorsEnum.Error.color;
            if (status === WhS_SupPL_ReceivedPricelistStatusEnum.WaitingConfirmation.value) return LabelColorsEnum.Processing.color;

            return LabelColorsEnum.Info.color;
        };

    }

    appControllers.service('WhS_SupPL_ReceivedSupplierPricelistService', ReceivedSupplierPricelistService);

})(appControllers);
