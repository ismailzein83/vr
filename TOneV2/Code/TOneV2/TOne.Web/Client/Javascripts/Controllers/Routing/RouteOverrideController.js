appControllers.controller('RouteOverrideController',
    function RouteOverrideController($scope, $http, CarriersService) {

        defineScopeObjects();
        defineScopeMethods();
        load();

        function defineScopeObjects() {
            $scope.optionsSuppliers = {
                selectedvalues: [],
                datasource: [],
                selectedgroup: false
            };

            $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };

        }
        function defineScopeMethods() {
            $scope.subViewConnector.getActionData = function () {
                return getActionData()
                
            }
            $scope.selectSupplier = function ($event, s) {
                $event.preventDefault();
                $event.stopPropagation();
                var index = null;
                try {
                    var index = $scope.findExsite($scope.optionsSuppliers.selectedvalues, s.CarrierAccountID, 'CarrierAccountID');
                }
                catch (e) {

                }
                if (index >= 0) {
                    $scope.optionsSuppliers.selectedvalues.splice(index, 1);
                }
                else
                    $scope.optionsSuppliers.selectedvalues.push(s);
            };

        }
        function load() {
            CarriersService.getSuppliers()
           .then(function (response) {
               $scope.optionsSuppliers.datasource = response;
               var tab = [];
               if ($scope.routeRule && $scope.routeRule.ActionData.$type=='TOne.LCR.Entities.OverrideRouteActionData, TOne.LCR.Entities') {                  
                   $.each($scope.routeRule.ActionData.Options, function (i, value) {
                       $scope.optionsSuppliers.selectedvalues.length = 0;
                       var existobj = $scope.findExsiteObj($scope.optionsSuppliers.datasource, value.SupplierId, 'CarrierAccountID')
                       if (existobj != null) {
                           tab[i] = {
                               CarrierAccountID: value.SupplierId,
                               Name: existobj.Name,
                               Percentage: $scope.routeRule.ActionData.Options[i].Percentage
                           }
                       }

                   });                 
                   $scope.optionsSuppliers.selectedvalues = tab;
               }

           })

        }          

        function getActionData() {
            return {
                $type: "TOne.LCR.Entities.OverrideRouteActionData, TOne.LCR.Entities",
                Options: fillOptionsData()
            }

        }
        function fillOptionsData() {
            var tab = [];
            $.each($scope.optionsSuppliers.selectedvalues, function (i, value) {
                tab[i] = {
                    SupplierId: value.CarrierAccountID,
                    Percentage: value.Percentage
                }

            });
            return tab;
        }

 });