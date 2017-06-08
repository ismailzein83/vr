(function (appControllers) {

    'use strict';

    CodeCompareService.$inject = ['VRModalService', 'LabelColorsEnum', ];

    function CodeCompareService(VRModalService, LabelColorsEnum) {

        function getStatusColor(saleCodeIndicator) {           
            if (saleCodeIndicator == 1) return LabelColorsEnum.DangerFont.color;
        };

        return {
            getStatusColor: getStatusColor
        };
    }

    appControllers.service('WhS_Sales_CodeCompareService', CodeCompareService);

})(appControllers);