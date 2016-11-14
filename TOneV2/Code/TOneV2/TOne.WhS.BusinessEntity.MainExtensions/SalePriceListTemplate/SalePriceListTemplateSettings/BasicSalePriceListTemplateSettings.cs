using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
	public class BasicSalePriceListTemplateSettings : SalePriceListTemplateSettings
	{
		public Guid ConfigId { get; set; }

		public long TemplateFileId { get; set; }

		public List<MappedSheet> MappedSheets { get; set; }

		public string DateTimeFormat { get; set; }

		public override byte[] Execute(ISalePriceListTemplateSettingsContext context)
		{
			var fileManager = new Vanrise.Common.Business.VRFileManager();
			Vanrise.Entities.VRFile file = fileManager.GetFile(TemplateFileId);
			if (file == null)
				throw new NullReferenceException(String.Format("File '{0}' was not found", TemplateFileId));
			if (file.Content == null)
				throw new NullReferenceException(String.Format("File '{0}' is empty", TemplateFileId));

			MemoryStream stream = new MemoryStream(file.Content);
			Workbook workbook = new Workbook(stream);

			Vanrise.Common.Utilities.ActivateAspose();

			SetWorkbookData(context, workbook);

			MemoryStream memoryStream = new MemoryStream();
			memoryStream = workbook.SaveToStream();
			return memoryStream.ToArray();
		}

		#region Private Methods

		private void SetWorkbookData(ISalePriceListTemplateSettingsContext context, Workbook workbook)
		{
			foreach (MappedSheet mappedSheet in MappedSheets)
			{
				Worksheet worksheet = workbook.Worksheets[mappedSheet.SheetIndex];
				SetWorksheetData(context, worksheet, mappedSheet);
			}
		}

		private void SetWorksheetData(ISalePriceListTemplateSettingsContext context, Worksheet worksheet, MappedSheet mappedSheet)
		{
			int currentRowIndex = mappedSheet.FirstRowIndex;

			foreach (SalePLZoneNotification zone in context.Zones)
			{
				foreach (var code in zone.Codes)
					if (!code.EED.HasValue)
						SetRecordData(worksheet, mappedSheet.MappedColumns, zone, code, zone.Rate, currentRowIndex++);
			}
		}

		private void SetRecordData(Worksheet worksheet, IEnumerable<MappedColumn> mappedCols, SalePLZoneNotification zone, SalePLCodeNotification code, SalePLRateNotification rate, int rowIndex)
		{
			foreach (MappedColumn mappedCol in mappedCols)
				SetCellData(worksheet, mappedCol, zone, code, rate, rowIndex);
		}

		private void SetCellData(Worksheet worksheet, MappedColumn mappedCol, SalePLZoneNotification zone, SalePLCodeNotification code, SalePLRateNotification rate, int rowIndex)
		{
			if (zone == null)
				throw new ArgumentNullException("zone");

			var mappedValueContext = new BasicSalePriceListTemplateSettingsMappedValueContext()
			{
				Zone = zone.ZoneName
			};

			if (code != null)
			{
				mappedValueContext.Code = code.Code;
				mappedValueContext.CodeBED = code.BED;
				mappedValueContext.CodeEED = code.EED;
			}

			if (rate != null)
			{
				mappedValueContext.Rate = rate.Rate;
				mappedValueContext.RateBED = rate.BED;
				mappedValueContext.RateEED = rate.EED;
			}

			mappedCol.MappedValue.Execute(mappedValueContext);

			if (mappedValueContext.Value != null && mappedValueContext.Value is DateTime)
				mappedValueContext.Value = ((DateTime)mappedValueContext.Value).ToString(DateTimeFormat);

			worksheet.Cells[rowIndex, mappedCol.ColumnIndex].PutValue(mappedValueContext.Value);
		}

		#endregion
	}
}
