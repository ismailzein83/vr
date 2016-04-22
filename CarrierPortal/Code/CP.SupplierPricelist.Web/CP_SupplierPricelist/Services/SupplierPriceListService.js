
app.service('CP_SupplierPricelist_SupplierPriceListService', ['LabelColorsEnum', 'CP_SupplierPricelist_PriceListResultEnum', 'CP_SupplierPricelist_PriceListStatusEnum',
    function (labelColorsEnum, cpSupplierPricelistResultEnum, cpSupplierPricelistStatusEnum) {
        function getSupplierPriceListStatusColor(value) {
            switch (value) {
                case cpSupplierPricelistStatusEnum.New.value:
                    return labelColorsEnum.New.color;
                case cpSupplierPricelistStatusEnum.Uploaded.value:
                    return labelColorsEnum.Success.color;
                case cpSupplierPricelistStatusEnum.WaitingReview.value:
                    return labelColorsEnum.WarningLevel1.color;
                case cpSupplierPricelistStatusEnum.Completed.value:
                    return labelColorsEnum.Success.color;
                case cpSupplierPricelistStatusEnum.UploadFailedWithRetry.value:
                    return labelColorsEnum.Failed.color;
                case cpSupplierPricelistStatusEnum.ResultFailedWithRetry.value:
                    return labelColorsEnum.WarningLevel2.color;
                case cpSupplierPricelistStatusEnum.UploadFailedWithNoRetry.value:
                    return labelColorsEnum.WarningLevel2.color;
                case cpSupplierPricelistStatusEnum.ResultFailedWithNoRetry.value:
                    return labelColorsEnum.Failed.color;
                case cpSupplierPricelistStatusEnum.UnderProcessing.value:
                    return labelColorsEnum.Success.color;
                default:
                    return undefined;
            }
        }

        function getSupplierPriceListResultColor(value) {
            switch (value) {
                case cpSupplierPricelistResultEnum.NotCompleted.value:
                    return labelColorsEnum.Failed.color;
                case cpSupplierPricelistResultEnum.Imported.value:
                    return labelColorsEnum.Success.color;
                case cpSupplierPricelistResultEnum.PartiallyApproved.value:
                    return labelColorsEnum.WarningLevel1.color;
                case cpSupplierPricelistResultEnum.Rejected.value:
                    return labelColorsEnum.Failed.color;
                default:
                    return undefined;
            }
        }

        return ({
            getSupplierPriceListStatusColor: getSupplierPriceListStatusColor,
            getSupplierPriceListResultColor: getSupplierPriceListResultColor
        });
    }]);
