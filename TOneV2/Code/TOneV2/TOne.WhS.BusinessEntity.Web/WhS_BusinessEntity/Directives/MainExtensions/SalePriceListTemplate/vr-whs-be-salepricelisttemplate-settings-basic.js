'use strict';

app.directive('vrWhsBeSalepricelisttemplateSettingsBasic', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
	return {
		restrict: "E",
		scope: {
			onReady: "=",
			normalColNum: '@',
			isrequired: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var basicSalePriceListTemplateSettings = new BasicSalePriceListTemplateSettings($scope, ctrl, $attrs);
			basicSalePriceListTemplateSettings.initializeController();
		},
		controllerAs: "basicSettingsCtrl",
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/SalePriceListTemplate/Templates/BasicSalePriceListTemplateSettingsTemplate.html'
	};

	function BasicSalePriceListTemplateSettings($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var excelWorkbookAPI;
		var excelWorkbookReadyDeferred = UtilsService.createPromiseDeferred();

		var firstRowDirectiveAPI;
		var firstRowDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

		var gridAPI;
		var gridReadyDeferred = UtilsService.createPromiseDeferred();

		function initializeController() {

			$scope.scopeModel = {};

			$scope.scopeModel.mappedCols = [];
			$scope.scopeModel.dateTimeFormat = 'd'; // Short date pattern

			$scope.scopeModel.onExcelWorkbookReady = function (api) {
				excelWorkbookAPI = api;
				excelWorkbookReadyDeferred.resolve();
			};

			$scope.scopeModel.onFirstRowMappingReady = function (api) {
				firstRowDirectiveAPI = api;
				firstRowDirectiveReadyDeferred.resolve();
			};

			$scope.scopeModel.onGridReady = function (api) {
				gridAPI = api;
				gridReadyDeferred.resolve();
			};

			$scope.scopeModel.addMappedCol = function () {
				var mappedCol = getMappedCol();
				$scope.scopeModel.mappedCols.push(mappedCol);
			};

			UtilsService.waitMultiplePromises([excelWorkbookReadyDeferred.promise, firstRowDirectiveReadyDeferred.promise, gridReadyDeferred.promise]).then(function () {
				defineAPI();
			});
		}
		function defineAPI() {

			var api = {};

			api.load = function (payload) {

				var promises = [];
				var settings;

				if (payload != undefined) {
					settings = payload.settings;
				}

				var mappedSheet;

				if (settings != undefined) {

					$scope.scopeModel.file = { fileId: settings.TemplateFileId };
					$scope.scopeModel.dateTimeFormat = settings.DateTimeFormat;

					if (settings.MappedSheets != null && settings.MappedSheets.length > 0)
						mappedSheet = settings.MappedSheets[0];
				}

				var loadFirstRowDirectivePromise = loadFirstRowDirective(mappedSheet);
				promises.push(loadFirstRowDirectivePromise);

				var loadMappedColumnsPromise = loadMappedColumns(mappedSheet);
				promises.push(loadMappedColumnsPromise);

				return UtilsService.waitMultiplePromises(promises);
			};

			api.getData = function getData() {

				var data = {
					$type: 'TOne.WhS.BusinessEntity.MainExtensions.BasicSalePriceListTemplateSettings, TOne.WhS.BusinessEntity.MainExtensions',
					TemplateFileId: $scope.scopeModel.file.fileId,
					DateTimeFormat: $scope.scopeModel.dateTimeFormat
				};
				
				var mappedSheet = getMappedSheet();
				data.MappedSheets = [mappedSheet];

				return data;
			};

			if (ctrl.onReady != null)
				ctrl.onReady(api);
		}

		function loadFirstRowDirective(mappedSheet) {

			var firstRowDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

			firstRowDirectiveReadyDeferred.promise.then(function () {
				var firstRowDirectivePayload = {
					context: getCellFieldMappingContext()
				};
				if (mappedSheet != undefined) {
					firstRowDirectivePayload.fieldMapping = {
						SheetIndex: mappedSheet.SheetIndex,
						RowIndex: mappedSheet.FirstRowIndex,
						CellIndex: 0
					};
				}
				VRUIUtilsService.callDirectiveLoad(firstRowDirectiveAPI, firstRowDirectivePayload, firstRowDirectiveLoadDeferred);
			});

			return firstRowDirectiveLoadDeferred.promise;
		}
		function loadMappedColumns(mappedSheet) {

			var promises = [];

			if (mappedSheet != undefined && mappedSheet.MappedColumns != null) {
				for (var i = 0; i < mappedSheet.MappedColumns.length; i++) {
					var mappedCol = getMappedCol(mappedSheet.MappedColumns[i], mappedSheet.SheetIndex, mappedSheet.FirstRowIndex);
					promises.push(mappedCol.directiveLoadDeferred.promise);
					$scope.scopeModel.mappedCols.push(mappedCol);
				}
			}

			return UtilsService.waitMultiplePromises(promises);
		}
		function getMappedCol(mappedColumn, sheetIndex, firstRowIndex) {

			if (mappedColumn != undefined)
				$scope.scopeModel.isLoadingMappedCol = true;

			var mappedCol = {};

			mappedCol.directiveLoadDeferred = UtilsService.createPromiseDeferred();

			mappedCol.onDirectiveReady = function (api) {
				mappedCol.directiveAPI = api;
				var directivePayload = {
					context: getCellFieldMappingContext()
				};
				if (mappedColumn != undefined) {
					directivePayload.fieldMapping = {
						SheetIndex: sheetIndex,
						RowIndex: firstRowIndex,
						CellIndex: mappedColumn.ColumnIndex
					};
				}
				VRUIUtilsService.callDirectiveLoad(mappedCol.directiveAPI, directivePayload, mappedCol.directiveLoadDeferred);
			};

			mappedCol.mappedValueSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

			mappedCol.onMappedValueSelectiveReady = function (api) {
				mappedCol.mappedValueSelectiveAPI = api;
				var mappedValueSelectivePayload;
				if (mappedColumn != undefined) {
					mappedValueSelectivePayload = { mappedValue: mappedColumn.MappedValue };
				}
				VRUIUtilsService.callDirectiveLoad(mappedCol.mappedValueSelectiveAPI, mappedValueSelectivePayload, mappedCol.mappedValueSelectiveLoadDeferred);
			};

			UtilsService.waitMultiplePromises([mappedCol.directiveLoadDeferred.promise, mappedCol.mappedValueSelectiveLoadDeferred.promise]).catch(function (error) {
				VRNotificationService.notifyException(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoadingMappedCol = false;
			});

			return mappedCol;
		};
		function getCellFieldMappingContext() {

			function selectCellAtSheet(sheetIndex, rowIndex, columnIndex) {
				var rowIndexAsInt = parseInt(rowIndex);
				var columnIndexAsInt = parseInt(columnIndex);
				if (excelWorkbookAPI.getSelectedSheetApi() != undefined)
					excelWorkbookAPI.selectCellAtSheet(rowIndex, columnIndex, sheetIndex);
			}
			function getSelectedCell() {
				var selectedSheetAPI = excelWorkbookAPI.getSelectedSheetApi();
				if (selectedSheetAPI != undefined)
					return selectedSheetAPI.getSelected();
			}
			function getSelectedSheet() {
				return excelWorkbookAPI.getSelectedSheet();
			}
			function getFirstRowIndex() {
				var firstRowDirectiveData = firstRowDirectiveAPI.getData();
				if (firstRowDirectiveData != undefined) {
					return {
						sheet: firstRowDirectiveData.SheetIndex,
						row: firstRowDirectiveData.RowIndex
					};
				}
			}

			return {
				setSelectedCell: selectCellAtSheet,
				getSelectedCell: getSelectedCell,
				getSelectedSheet: getSelectedSheet,
				getFirstRowIndex: getFirstRowIndex
			};
		}

		function getMappedSheet() {
			
			var firstRowDirectiveData = firstRowDirectiveAPI.getData();
			if (firstRowDirectiveData == undefined)
				return null;

			var mappedSheet = {
				SheetIndex: firstRowDirectiveData.SheetIndex,
				FirstRowIndex: firstRowDirectiveData.RowIndex,
				MappedColumns: getMappedColumns()
			};
			return mappedSheet;
		}
		function getMappedColumns() {

			if ($scope.scopeModel.mappedCols.length == 0)
				return null;

			var mappedColumns = [];

			for (var i = 0; i < $scope.scopeModel.mappedCols.length; i++) {

				var mappedCol = $scope.scopeModel.mappedCols[i];
				var mappedColumn = {};

				var directiveData = mappedCol.directiveAPI.getData();
				if (directiveData != undefined)
					mappedColumn.ColumnIndex = directiveData.CellIndex;

				mappedColumn.MappedValue = mappedCol.mappedValueSelectiveAPI.getData();

				mappedColumns.push(mappedColumn);
			}

			return mappedColumns;
		}
	}
}]);