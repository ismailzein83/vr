appControllers.controller('RouteRuleEditorController2',
    function RouteRuleEditorController2($scope, $http, $location, $routeParams, notify) {


        defineScopeObjects();
        defineScopeMethods();
        load();

        function defineScopeObjects() {
            $scope.subViewConnector = {};
            $scope.BEDDate = "";
            $scope.EEDDate = "";
            $scope.optionsRouteType = {
                selectedvalues: [],
                datasource: [],
                lastselectedvalue: ''
            };
            $scope.optionsEditorType = {
                selectedvalues: [],
                datasource: [],
                lastselectedvalue: ''
            };
            $scope.optionsRuleType = {
                selectedvalues: [],
                datasource: [],
                lastselectedvalue: ""
            };          
            $scope.optionsRouteType.datasource = [
                 { name: 'Override Route', url: '/Client/Templates/PartialTemplate/RouteOverrideTemplate2.html', objectType: 'TOne.LCR.Entities.OverrideRouteActionData, TOne.LCR.Entities' },
                 { name: 'Priority Rule', url: '/Client/Templates/PartialTemplate/PriorityTemplate2.html', objectType: 'TOne.LCR.Entities.PriorityRouteActionData, TOne.LCR.Entities' },
                 { name: 'Block Route', url: '/Client/Templates/PartialTemplate/RouteBlockTemplate.html', objectType: 'TOne.LCR.Entities.BlockRouteActionData, TOne.LCR.Entities' }
            ]

            $scope.optionsEditorType.datasource = [
                 { name: 'Zone', url: '/Client/Templates/PartialTemplate/ZoneTemplate2.html', objectType: 'TOne.LCR.Entities.ZoneSelectionSet, TOne.LCR.Entities' },
                 { name: 'Code', url: '/Client/Templates/PartialTemplate/CodeTemplate.html', objectType: 'TOne.LCR.Entities.CodeSelectionSet, TOne.LCR.Entities' }
            ]
            $scope.optionsRuleType.datasource = [
              { name: 'Customer', url: '/Client/Templates/PartialTemplate/CustomerTemplate2.html' },
              { name: 'Pool', url: '/Client/Templates/PartialTemplate/PoolTemplate.html' },
              { name: 'Product', url: '/Client/Templates/PartialTemplate/ProductTemplate.html' }
            ]


        }
        function defineScopeMethods() {
            $scope.muteAction = function (e) {
                e.preventDefault();
                e.stopPropagation();
            }

            $scope.isvalidcomp = function (model) {
                var s = "required-inpute";
                if ($scope[model] != undefined) {
                    s = ($scope[model].url != '') ? "" : "required-inpute";
                }
                return s;
            }
            $scope.isvalidcompDate = function (model) {
                var s = "required-inpute";
                if ($scope[model] != undefined && $scope[model] != "") {
                    var d = $scope[model];
                    s = ($scope.isDate(d)) ? "" : "required-inpute";
                }
                return s;


            }
            $scope.isvalidcompDateEED = function () {
                if (typeof ($scope.EEDDate) == "object" || typeof ($scope.EEDDate) == "string")
                    return "";
                else if (typeof ($scope.EEDDate) == "undefined")
                    return ($scope.isDate($scope.EEDDate)) ? "" : "required-inpute";


            }
            $scope.initErrortooltipBED = function () {
                if ($scope.showtd == true) {
                    if (!$scope.isDate($scope.BEDDate)) {
                        var msg = "";
                        if (typeof ($scope.BEDDate) == "object" || typeof ($scope.BEDDate) == "string")
                            msg = "Date is required.";
                        if (typeof ($scope.BEDDate) == "undefined")
                            msg = "Invalide format.";

                        $scope.msgd = msg

                        return true;
                    }
                }
            }
            $scope.initErrortooltipEED = function () {
                if ($scope.showtde == true) {
                    var msg = "";
                    if (typeof ($scope.EEDDate) == "undefined") {
                        msg = "Invalide format.";
                        $scope.msged = msg
                        return true;
                    }
                    else if (typeof ($scope.BEDDate) == "object" || typeof ($scope.BEDDate) == "string")
                        return false

                }
            }
            $scope.validateForm = function () {
                var obj = ($scope.subViewConnector.getCarrierAccountSet != undefined) ? $scope.subViewConnector.getCarrierAccountSet() : null;

                return ($scope.optionsRouteType.lastselectedvalue.url == '') || (!$scope.isDate($scope.dateToString($scope.BEDDate))) || $scope.isvalidcompDateEED() != "" || (obj == null || obj.Customers == undefined || obj.Customers.SelectedValues.length == 0);
            }

            $scope.update = function (val, model) {
                if (model == 'routetype' && val.name != 'Customer') {
                    $scope[model] = { "name": 'Customer', "url": '/Client/Templates/PartialTemplate/CustomerTemplate2.html' };
                    notify.closeAll();
                    notify({ message: 'This module is under construction.', classes: "alert alert-danger" });
                }
                else {
                    $scope[model] = val;
                }

            }
            $scope.update2 = function (data, items, last) {
                if (last.name != 'Customer') {

                    notify.closeAll();
                    notify({ message: 'This module is under construction.', classes: "alert alert-danger" });
                    return $scope.optionsRuleType.datasource[0];
                }
            }
            $scope.saveRule = function () {
                var routeRule =
                    {
                        CodeSet: $scope.subViewConnector.getCodeSet(),
                        CarrierAccountSet: $scope.subViewConnector.getCarrierAccountSet(),
                        ActionData: $scope.subViewConnector.getActionData(),
                        Type: "RouteRule",
                        BeginEffectiveDate: $scope.BEDDate,
                        EndEffectiveDate: ($scope.EEDDate),
                        Reason: $scope.Reason
                    };
                $http.post($scope.baseurl + "/api/routing/SaveRouteRule",
                             routeRule)
                         .success(function (response) {
                             notify({ message: 'Route Rule has been saved successfully.', classes: "alert  alert-success" });
                             $location.path("/RouteRuleManager").replace();
                         });
            }
            $scope.cancel = function () {
                $location.path("/RouteRuleManager").replace();
            };


        }
        function load() {
            
            $('.dropdown').on('show.bs.dropdown', function (e) {
                $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
            });

            $('.dropdown').on('hide.bs.dropdown', function (e) {
                $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
            });

            var dropdownHidingTimeoutHandler;

            $('.dropdown-custom').on('mouseenter', function () {
                var $this = $(this);
                clearTimeout(dropdownHidingTimeoutHandler);
                if (!$this.hasClass('open')) {
                    $('.dropdown-toggle', $this).dropdown('toggle');
                }
            });

            $('.dropdown-custom').on('mouseleave', function () {
                var $this = $(this);
                dropdownHidingTimeoutHandler = setTimeout(function () {
                    if ($this.hasClass('open')) {
                        $('.dropdown-toggle', $this).dropdown('toggle');
                    }
                }, 150);
            });

            if ($routeParams.RouteRuleId != 'undefined') {
                $http.get($scope.baseurl + "/api/Routing/GetRouteRuleDetails",
                {
                    params: {
                        RouteRuleId: $routeParams.RouteRuleId
                    }
                })
                 .success(function (response) {
                     $scope.routeRule = response;
                     $scope.optionsRuleType.lastselectedvalue = $scope.optionsRuleType.datasource[0];
                     $scope.BEDDate = new Date($scope.routeRule.BeginEffectiveDate);
                     $scope.EEDDate = $scope.routeRule.EndEffectiveDate;
                     $scope.Reason = $scope.routeRule.Reason;
                     $scope.optionsRouteType.lastselectedvalue = null;
                     $scope.optionsRouteType.lastselectedvalue = $scope.optionsRouteType.datasource[$scope.findExsite($scope.optionsRouteType.datasource, $scope.routeRule.ActionData.$type, 'objectType')];
                     $scope.optionsEditorType.lastselectedvalue = null;
                     $scope.optionsEditorType.lastselectedvalue = $scope.optionsEditorType.datasource[$scope.findExsite($scope.optionsEditorType.datasource, $scope.routeRule.CodeSet.$type, 'objectType')];

                 });
            }
            else {
                $scope.optionsRouteType.lastselectedvalue = { name: 'Select ..', url: '' };
                $scope.optionsEditorType.lastselectedvalue = $scope.optionsEditorType.datasource[0];
                $scope.optionsRuleType.lastselectedvalue = $scope.optionsRuleType.datasource[0];
            }

        }
});

