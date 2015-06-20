appControllers.controller('RoutingRules_RouteOverrideTemplateController',
    function RouteOverrideController($scope, $http, CarrierAPIService, RoutingRulesTemplatesEnum, UtilsService, CarrierTypeEnum) {

        defineScope();
        load();

        function defineScope() {
            $scope.suppliers = [];
            $scope.selectedSuppliers = [];

            $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };

            $scope.subViewConnector.getActionData = function () {
                return getActionData()

            }
            $scope.selectSupplier = function ($event, s) {
                $event.preventDefault();
                $event.stopPropagation();
                var index = null;
                try {
                    var index = UtilsService.getItemIndexByVal($scope.selectedSuppliers, s.CarrierAccountID, 'CarrierAccountID');
                }
                catch (e) {

                }
                if (index >= 0) {
                    $scope.selectedSuppliers.splice(index, 1);
                }
                else
                    $scope.selectedSuppliers.push(s);
            };

        }

        function load() {
            CarrierAPIService.GetCarriers(CarrierTypeEnum.Supplier.value)
           .then(function (response) {
               $scope.suppliers = response;
               var tab = [];
               if ($scope.routeRule && $scope.routeRule.ActionData.$type == RoutingRulesTemplatesEnum.OverrideTemplate.objectType) {
                   $.each($scope.routeRule.ActionData.Options, function (i, value) {
                       $scope.selectedSuppliers.length = 0;
                       var existobj = UtilsService.getItemByVal($scope.suppliers, value.SupplierId, 'CarrierAccountID')
                       if (existobj != null) {
                           tab[i] = {
                               CarrierAccountID: value.SupplierId,
                               Name: existobj.Name,
                               AllowLoss: $scope.routeRule.ActionData.Options[i].AllowLoss,
                               Percentage: $scope.routeRule.ActionData.Options[i].Percentage
                           }
                       }

                   });
                   $scope.selectedSuppliers = tab;
               }

           })

        }

        function getActionData() {
            return {
                $type: RoutingRulesTemplatesEnum.OverrideTemplate.objectType,
                Options: fillOptionsData()
            }

        }

        function fillOptionsData() {
            var tab = [];
            $.each($scope.selectedSuppliers, function (i, value) {
                tab[i] = {
                    SupplierId: value.CarrierAccountID,
                    Percentage: value.Percentage,
                    AllowLoss: value.AllowLoss
                }

            });
            return tab;
        }

    });