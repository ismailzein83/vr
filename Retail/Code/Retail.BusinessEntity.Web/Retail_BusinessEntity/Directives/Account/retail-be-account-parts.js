'use strict';

app.directive('retailBeAccountParts', ['Retail_BE_AccountPartDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', function (Retail_BE_AccountPartDefinitionAPIService, UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountParts = new AccountParts($scope, ctrl, $attrs);
            accountParts.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Account/Templates/AccountPartsTemplate.html'
    };

    function AccountParts($scope, ctrl, $attrs)
    {
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.parts = [];

            defineAPI();
        }
        function defineAPI()
        {
            var api = {};

            api.load = function (payload)
            {
                $scope.scopeModel.parts.length = 0;

                var context;
                var partDefinitions;
                var parts;

                if (payload != undefined) {
                    context = payload.context;
                    partDefinitions = payload.partDefinitions;
                    parts = payload.parts;
                }

                if (partDefinitions == undefined)
                    return;

                var promises = [];

                for (var i = 0; i < partDefinitions.length; i++)
                {
                    var partDefinition = partDefinitions[i];
                    var part = buildPart(partDefinition);
                    promises.push(part.directiveLoadDeferred.promise);
                    $scope.scopeModel.parts.push(part);
                }

                function buildPart(partDefinition)
                {
                    var part = {};
                    part.definitionId = partDefinition.AccountPartDefinitionId;
                    part.runtimeEditor = context.getPartDefinitionRuntimeEditor(partDefinition.Settings.ConfigId);
                    part.title = partDefinition.Title;
                    part.directiveLoadDeferred = UtilsService.createPromiseDeferred();
                    part.onDirectiveReady = function (api) {
                        part.directiveAPI = api;
                        var directivePayload = {
                            partDefinition: partDefinition,
                            partSettings: (parts != undefined && parts[partDefinition.AccountPartDefinitionId] != undefined) ?
                                parts[partDefinition.AccountPartDefinitionId].Settings : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(part.directiveAPI, directivePayload, part.directiveLoadDeferred);
                    };
                    return part;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function ()
            {
                if ($scope.scopeModel.parts.length == 0)
                    return;

                var parts = {};

                for (var i = 0; i < $scope.scopeModel.parts.length; i++)
                {
                    var part = $scope.scopeModel.parts[i];
                    var partData = {};
                    partData.Settings = part.directiveAPI.getData();
                    parts[part.definitionId] = partData;
                }

                return parts;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
