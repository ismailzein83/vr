appControllers.controller('RoutingRules_PriorityTemplateController',
    function PriorityController($scope, $http, CarrierAPIService, RoutingRulesTemplatesEnum, UtilsService, CarrierTypeEnum) {

        defineScope();
        load();

        function defineScope() {
            $scope.suppliers = [];
            $scope.selectedSuppliers = [];
            $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };

            $scope.subViewConnector.getActionData = function () {
                return getActionData();
            }

            $scope.fillOptionsData = function () {
                var tab = [];
                $.each($scope.selectedSuppliers, function (i, value) {
                    tab[i] = {
                        SupplierId: value.CarrierAccountID,
                        Priority: ($scope.selectedSuppliers.length) - i,
                        Force: (value.Force == true) ? true : false,
                        Percentage: value.Percentage
                    }

                });
                return tab;
            }

        }

        function load() {

            CarrierAPIService.GetCarriers(CarrierTypeEnum.Supplier.value)
                .then(function (response) {
                    $scope.suppliers = response;
                    $scope.selectedSuppliers.length = 0;
                    var tab = [];
                    if ($scope.routeRule && $scope.routeRule.ActionData.$type == RoutingRulesTemplatesEnum.PriorityTemplate.objectType) {

                        $.each($scope.routeRule.ActionData.Options, function (i, value) {
                            var existobj = UtilsService.getItemByVal($scope.suppliers, value.SupplierId, 'CarrierAccountID')
                            if (existobj != null) {
                                tab[i] = {
                                    CarrierAccountID: value.SupplierId,
                                    Name: existobj.Name,
                                    Force: value.Force,
                                    Percentage: $scope.routeRule.ActionData.Options[i].Percentage,
                                    Priority: value.Priority
                                }
                            }

                        });
                        $scope.selectedSuppliers = tab;
                    }

                })

            $scope.selectSupplier = function ($event, s) {
                $event.preventDefault();
                $event.stopPropagation();

                var index = UtilsService.getItemIndexByVal($scope.selectedSuppliers, s.CarrierAccountID, 'CarrierAccountID');

                if (index >= 0) {
                    $scope.selectedSuppliers.splice(index, 1);
                }
                else {
                    $scope.selectedSuppliers.push(s);
                }

            };

        }

        function getActionData() {
            return {
                $type: RoutingRulesTemplatesEnum.PriorityTemplate.objectType,
                Options: $scope.fillOptionsData()
            }
        }

    });