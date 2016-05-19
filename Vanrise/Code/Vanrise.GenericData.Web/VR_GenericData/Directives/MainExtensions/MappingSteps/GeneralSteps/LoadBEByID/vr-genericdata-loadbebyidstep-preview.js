'use strict';

app.directive('vrGenericdataLoadbebyidstepPreview', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {

    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var loadBEByIDStepPreview = new LoadBEByIDStepPreview(ctrl, $scope);
            loadBEByIDStepPreview.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/LoadBEByID/Templates/LoadBEByIDStepPreviewTemplate.html'
    };

    function LoadBEByIDStepPreview(ctrl, $scope)
    {
        this.initializeController = initializeController;

        var stepObj = {};

        function initializeController() {
            defineAPI();
        }

        function defineAPI()
        {
            var api = {};

            api.load = function (payload)
            {
                if (payload != undefined)
                {
                    if (payload.stepDetails != undefined)
                    {
                        stepObj.stepDetails = payload.stepDetails;
                        ctrl.businessEntityDefinitionId = payload.stepDetails.BusinessEntityDefinitionId;
                        ctrl.businessEntityId = payload.stepDetails.BusinessEntityId;
                        ctrl.businessEntity = payload.stepDetails.BusinessEntity;
                    }
                    checkValidation();
                }
            };

            api.applyChanges = function (changes)
            {
                ctrl.businessEntityDefinitionId = changes.BusinessEntityDefinitionId;
                ctrl.businessEntityId = changes.BusinessEntityId;
                ctrl.businessEntity = changes.BusinessEntity;
                stepObj.stepDetails = changes;
            };

            api.checkValidation = function () {
                return checkValidation();
            };

            api.getData = function () {
                return stepObj.stepDetails;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function checkValidation()
        {
            if (ctrl.businessEntityDefinitionId == undefined)
                return 'BusinessEntityDefinitionId is required';
            if (ctrl.businessEntityId == undefined)
                return 'BusinessEntityId is required';
            if (ctrl.businessEntity == undefined)
                return 'BusinessEntity is required';
            return null;
        }
    }
}]);