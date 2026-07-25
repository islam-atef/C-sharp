using System;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Huawei_Commissioning_App.Classes.Models;

namespace Huawei_Commissioning_App.Classes.Strategies
{
    public class ExcelCabinetInputStrategy : ICabinetInputStrategy
    {
        private readonly string _filePath;

        public ExcelCabinetInputStrategy(string filePath)
        {
            _filePath = Path.GetFullPath(filePath);
        }

        public IEnumerable<CabinetInfo> GetCabinets()
        {
            var cabinets = new Queue<CabinetInfo>();
            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"Cabinet input file not found: {_filePath}");
                return cabinets;
            }

            using (FileStream file = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook? workbook = new XSSFWorkbook(file);
                if (workbook != null)
                {
                    ISheet sheet = workbook.GetSheetAt(0);
                    int rowIndex = 1; // start from row 1, as row 0 is the header.
                    IRow row = sheet.GetRow(rowIndex);
                    
                    while (row != null && row.GetCell(0) != null)
                    {
                        var cabinet = new CabinetInfo
                        {
                            CabinetFamilyName = row.GetCell(0)?.ToString(),
                            CabinetType = row.GetCell(1)?.ToString(),
                            Code1 = row.GetCell(2)?.ToString()
                        };

                        if (cabinet.Code1 != null)
                        {
                            cabinet.Code1 = cabinet.Code1.Trim();
                        }

                        if (cabinet.CabinetType == "MA5818")
                        {
                            cabinet.Code2 = row.GetCell(3)?.ToString()?.Trim();
                        }

                        cabinets.Enqueue(cabinet);
                        rowIndex++;
                        row = sheet.GetRow(rowIndex);
                    }
                }
            }
            return cabinets;
        }
    }
}
