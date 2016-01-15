
app.service('CP_SupplierPricelist_SupplierPriceListService', ['LabelColorsEnum', 'CP_SupplierPricelist_PriceListResultEnum', 'CP_SupplierPricelist_PriceListStatusEnum',
    function (labelColorsEnum, cpSupplierPricelistResultEnum, cpSupplierPricelistStatusEnum) {
        function getSupplierPriceListStatusColor(value) {
            switch (value) {
            case cpSupplierPricelistStatusEnum.New.value:
                return labelColorsEnum.New.color;
                break;
            case cpSupplierPricelistStatusEnum.Initiated.value:
                return labelColorsEnum.Primary.color;
                break;
            case cpSupplierPricelistStatusEnum.InitiationFailedWithRetry.value:
                return labelColorsEnum.Warning.color;
                break;
            case cpSupplierPricelistStatusEnum.InitiationFailedWithNoRetry.value:
                return labelColorsEnum.WarningLevel2.color;
                break;
            case cpSupplierPricelistStatusEnum.PartiallyCompleted.value:
                return labelColorsEnum.Processing.color;
                break;
            case cpSupplierPricelistStatusEnum.Completed.value:
                return labelColorsEnum.Success.color;
                break;
            case cpSupplierPricelistStatusEnum.GetProgressFailedWithRetry.value:
                return labelColorsEnum.WarningLevel1.color;
                break;
            case cpSupplierPricelistStatusEnum.GetProgressFailedWithNoRetry.value:
                return labelColorsEnum.Failed.color;
                break;
            default:
                return undefined;
            }
        }

        function getSupplierPriceListResultColor(value) {
            switch (value) {
            case cpSupplierPricelistResultEnum.NotCompleted.value:
                return labelColorsEnum.Error.color;
                break;
            case cpSupplierPricelistResultEnum.Succeeded.value:
                return labelColorsEnum.Success.color;
                break;
            case cpSupplierPricelistResultEnum.PartiallySucceeded.value:
                return labelColorsEnum.WarningLevel1.color;
                break;
            case cpSupplierPricelistResultEnum.Failed.value:
                return labelColorsEnum.Failed.color;
                break;
            case cpSupplierPricelistResultEnum.NotAnswered.value:
                return labelColorsEnum.Warning.color;
                break;
            default:
                return undefined;
            }
        }

        return ({
            getSupplierPriceListStatusColor: getSupplierPriceListStatusColor,
            getSupplierPriceListResultColor: getSupplierPriceListResultColor
        });
    }]);
