using System;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Huawei_Commissioning_App.Classes.Models;

namespace Huawei_Commissioning_App.Classes.Strategies
{
    public class ExcelIpProviderStrategy : IIpProviderStrategy
    {
        private readonly string[] _filePaths;

        public ExcelIpProviderStrategy(string[] filePaths)
        {
            _filePaths = new string[filePaths.Length];
            for (int i = 0; i < filePaths.Length; i++)
            {
                _filePaths[i] = Path.GetFullPath(filePaths[i]);
            }
        }

        public bool GetIPs(IpPlan T, string? Cab_Code)
        {
            bool cabinetStatus = false;
            for (int i = 0; i < _filePaths.Length; i++)
            {
                if (!File.Exists(_filePaths[i]))
                {
                    continue;
                }

                using (FileStream file = new FileStream(_filePaths[i], FileMode.Open, FileAccess.Read))
                {
                    IWorkbook? workbook = new XSSFWorkbook(file);
                    if (workbook != null)
                    {
                        ISheet sheet = workbook.GetSheetAt(0);
                        for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                        {
                            IRow row = sheet.GetRow(rowIndex);
                            if (row != null)
                            {
                                ICell cell = row.GetCell(0);
                                if (cell != null && cell.ToString() == Cab_Code)
                                {
                                    cabinetStatus = true;
                                    if (i == 0)
                                    {
                                        T.SigGatewayIp = row.GetCell(2)?.ToString();
                                        T.SigSH1Ip = row.GetCell(3)?.ToString();
                                        T.SigSH2Ip = row.GetCell(4)?.ToString();
                                        T.MgGatewayIp = row.GetCell(8)?.ToString();
                                        T.MgSH1Ip = row.GetCell(9)?.ToString();
                                        T.MgSH2Ip = row.GetCell(10)?.ToString();
                                        T.MgSH3Ip = row.GetCell(11)?.ToString();
                                        T.FvnoEmGatewayIp = row.GetCell(14)?.ToString();
                                        T.FvnoEmSH1Ip = row.GetCell(15)?.ToString();
                                        T.FvnoEmSH2Ip = row.GetCell(16)?.ToString();
                                    }
                                    else
                                    {
                                        T.PopName = row.GetCell(2)?.ToString();
                                        T.TedMgGatewayIp = row.GetCell(9)?.ToString();
                                        T.TedMgSH1Ip = row.GetCell(10)?.ToString();
                                        T.TedMgSH2Ip = row.GetCell(11)?.ToString();
                                    }
                                    break;
                                }
                                else
                                {
                                    cabinetStatus = false;
                                }
                            }
                        }
                    }
                }
                if (cabinetStatus == false)
                    break;
            }
            return cabinetStatus;
        }
    }
}
